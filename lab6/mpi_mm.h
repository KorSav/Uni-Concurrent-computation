#ifndef MPI_MATRIX_MULTIPLICATION_H
#define MPI_MATRIX_MULTIPLICATION_H

#include <stdio.h>
#include <stdlib.h>
#include <mpi.h>

#define MASTER 0
#define FROM_MASTER 1
#define FROM_WORKER 2
#define MAX_SEND_COUNT_PER_WORKER 4

typedef struct _params_nb
{
    const double *const a, *const b;
    double *const c;
    int nra, nca, ncb;
    char verbose;
    int *offsets, *prows;
    MPI_Request *requests;
    int numtasks, numworkers;
} _params_nb;

void __mpi_mm(const double *const, const double *const, double *const,
              int, int, int, char, int);
void fill_offsets_and_rows(_params_nb *);
void master_send_nb(_params_nb *);
void master_receive_nb(_params_nb *);
void master_send(_params_nb *);
void master_receive(_params_nb *);
void worker_process_data(int, int);
void fill_matrix(double *, double, int, int);

// Workers should not allocate any matrices, null pointers can be passed
void mpi_mm(const double *const a,
            const double *const b,
            double *const c,
            int nra, int nca, int ncb,
            char verbose)
{
    __mpi_mm(a, b, c, nra, nca, ncb, verbose, 1);
}

// Workers should not allocate any matrices, null pointers can be passed
void mpi_mm_nb(const double *const a,
               const double *const b,
               double *const c,
               int nra, int nca, int ncb,
               char verbose)
{
    __mpi_mm(a, b, c, nra, nca, ncb, verbose, 0);
}

void __mpi_mm(const double *const a,
              const double *const b,
              double *const c,
              int nra, int nca, int ncb,
              char verbose, int isBlocking)
{
    int numtasks, taskid;

    MPI_Comm_size(MPI_COMM_WORLD, &numtasks);
    MPI_Comm_rank(MPI_COMM_WORLD, &taskid);
    if (numtasks < 2)
    {
        fprintf(stderr, "Need at least two MPI tasks. Quitting...\n");
        MPI_Abort(MPI_COMM_WORLD, 1);
    }

    if (taskid == MASTER)
    {
        _params_nb params = {a, b, c, nra, nca, ncb, verbose};
        params.numtasks = numtasks;
        params.numworkers = params.numtasks - 1;
        if (!isBlocking)
            params.requests = (MPI_Request *)malloc(params.numworkers * MAX_SEND_COUNT_PER_WORKER * sizeof(MPI_Request));
        params.prows = (int *)malloc(params.numworkers * sizeof(int));
        params.offsets = (int *)malloc(params.numworkers * sizeof(int));
        fill_offsets_and_rows(&params);

        if (!isBlocking)
        {
            master_send_nb(&params);
            master_receive_nb(&params);
        }
        else
        {
            master_send(&params);
            master_receive(&params);
        }
        free(params.requests);
        free(params.prows);
        free(params.offsets);
    }
    else
    {
        worker_process_data(nca, ncb);
    }
}

void fill_offsets_and_rows(_params_nb *ps)
{
    int i, averow, extra;

    averow = ps->nra / ps->numworkers;
    extra = ps->nra % ps->numworkers;
    ps->offsets[0] = 0;
    for (i = 0; i < ps->numworkers - 1; i++)
    {
        ps->prows[i] = (i < extra) ? averow + 1 : averow;
        ps->offsets[i + 1] = ps->offsets[i] + ps->prows[i];
    }
    ps->prows[ps->numworkers - 1] = averow;
}

void master_send(_params_nb *ps)
{
    int i, dest, rows, offset;

    for (i = 0; i < ps->numworkers; i++)
    {
        dest = i + 1;
        rows = ps->prows[i];
        offset = ps->offsets[i];
        if (ps->verbose)
        {
            printf("Send %d rows to task %d offset= %d\n",
                   rows, dest, offset);
        }
        MPI_Send(&rows, 1, MPI_INT, dest,
                 FROM_MASTER, MPI_COMM_WORLD);
        MPI_Send(ps->a + offset * ps->nca, rows * ps->nca, MPI_DOUBLE, dest,
                 FROM_MASTER, MPI_COMM_WORLD);
        MPI_Send(ps->b, ps->nca * ps->ncb, MPI_DOUBLE, dest,
                 FROM_MASTER, MPI_COMM_WORLD);
    }
    if (ps->verbose)
    {
        printf("\n");
    }
}

void master_send_nb(_params_nb *ps)
{
    int i, dest, base, rows, offset;
    const int mpiCallsCount = 3;
    MPI_Status status;

    for (i = 0; i < ps->numworkers; i++)
    {
        dest = i + 1;
        base = i * mpiCallsCount;
        rows = ps->prows[i];
        offset = ps->offsets[i];
        if (ps->verbose)
        {
            printf("Start incomplete sending %d rows to task %d offset= %d\n",
                   rows, dest, offset);
        }
        MPI_Isend(&rows, 1, MPI_INT, dest,
                  FROM_MASTER, MPI_COMM_WORLD, ps->requests + base + 0);
        MPI_Isend(ps->a + offset * ps->nca, rows * ps->nca, MPI_DOUBLE, dest,
                  FROM_MASTER, MPI_COMM_WORLD, ps->requests + base + 1);
        MPI_Isend(ps->b, ps->nca * ps->ncb, MPI_DOUBLE, dest,
                  FROM_MASTER, MPI_COMM_WORLD, ps->requests + base + 2);
    }
    for (i = 0; i < ps->numworkers; i++)
    {
        dest = i + 1;
        base = i * mpiCallsCount;
        if (ps->verbose)
        {
            printf("Wait upon sending completion for task %d\n", dest);
        }
        MPI_Wait(ps->requests + base + 0, &status);
        MPI_Wait(ps->requests + base + 1, &status);
        MPI_Wait(ps->requests + base + 2, &status);
    }
    if (ps->verbose)
    {
        printf("\n");
    }
}

void master_receive(_params_nb *ps)
{
    int i, source, offset, rows;
    MPI_Status status;

    for (i = 0; i < ps->numworkers; i++)
    {
        source = i + 1;
        offset = ps->offsets[i];
        rows = ps->prows[i];
        MPI_Recv(ps->c + offset * ps->ncb, rows * ps->ncb, MPI_DOUBLE,
                 source, FROM_WORKER,
                 MPI_COMM_WORLD, &status);
        if (ps->verbose)
        {
            printf("Recieve results from task %d\n", source);
        }
    }
}

void master_receive_nb(_params_nb *ps)
{
    int i, source, base, offset, rows;
    MPI_Status status;

    for (i = 0; i < ps->numworkers; i++)
    {
        source = i + 1;
        base = i;
        offset = ps->offsets[i];
        rows = ps->prows[i];
        MPI_Irecv(ps->c + offset * ps->ncb, rows * ps->ncb, MPI_DOUBLE,
                  source, FROM_WORKER,
                  MPI_COMM_WORLD, ps->requests + base + 0);
        if (ps->verbose)
        {
            printf("Start receiving results from task %d\n", source);
        }
    }
    for (i = 0; i < ps->numworkers; i++)
    {
        source = i + 1;
        base = i;
        if (ps->verbose)
        {
            printf("Wait upon receive completion for task %d\n", source);
        }
        MPI_Wait(ps->requests + base + 0, &status);
    }
}

void worker_process_data(int nca, int ncb)
{
    int rows, k, i, j;
    MPI_Status status;
    double (*a)[nca];
    double (*b)[ncb];
    double (*c)[ncb];

    MPI_Recv(&rows, 1, MPI_INT, MASTER, FROM_MASTER,
             MPI_COMM_WORLD, &status);

    a = (double (*)[nca])malloc(rows * nca * sizeof(double));
    MPI_Recv(a, rows * nca, MPI_DOUBLE, MASTER, FROM_MASTER,
             MPI_COMM_WORLD, &status);

    b = (double (*)[ncb])malloc(nca * ncb * sizeof(double));
    MPI_Recv(b, nca * ncb, MPI_DOUBLE, MASTER, FROM_MASTER,
             MPI_COMM_WORLD, &status);

    c = (double (*)[ncb])malloc(rows * ncb * sizeof(double));
    for (k = 0; k < ncb; k++)
        for (i = 0; i < rows; i++)
        {
            c[i][k] = 0.0;
            for (j = 0; j < nca; j++)
                c[i][k] = c[i][k] + a[i][j] * b[j][k];
        }

    MPI_Send(c, rows * ncb, MPI_DOUBLE, MASTER,
             FROM_WORKER, MPI_COMM_WORLD);
    free(a);
    free(b);
    free(c);
}

void fill_matrix(double *m, double val, int nr, int nc)
{
    int i;
    for (i = 0; i < nr * nc; i++)
        m[i] = val;
}

#endif // MPI_MATRIX_MULTIPLICATION_H
