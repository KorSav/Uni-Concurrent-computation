#include "mpi.h"
#include <stdio.h>
#include <stdlib.h>
#define NRA 62        /* number of rows in matrix A */
#define NCA 15        /* number of columns in matrix A */
#define NCB 7         /* number of columns in matrix B */
#define MASTER 0      /* taskid of first task */
#define FROM_MASTER 1 /* setting a message type */
#define FROM_WORKER 2 /* setting a message type */
int main(int argc, char *argv[])
{
    int numtasks,
        taskid,
        numworkers,
        source,
        dest,
        rows, /* rows of matrix A sent to each worker */
        averow, extra, offset,
        i, j, k, rc;
    double a[NRA][NCA], /* matrix A to be multiplied */
        b[NCA][NCB],    /* matrix B to be multiplied */
        c[NRA][NCB];    /* result matrix C */
    MPI_Status status;
    int base;
    const int maxRequestsCountPerCommunicator = 4;
    int *offsets;
    int *prows;
    MPI_Request *requests;
    MPI_Init(&argc, &argv);
    MPI_Comm_size(MPI_COMM_WORLD, &numtasks);
    MPI_Comm_rank(MPI_COMM_WORLD, &taskid);
    if (numtasks < 2)
    {
        printf("Need at least two MPI tasks. Quitting...\n");
        MPI_Abort(MPI_COMM_WORLD, rc);
        exit(1);
    }
    numworkers = numtasks - 1;
    if (taskid == MASTER)
    {
        printf("mpi_mm has started with %d tasks.\n", numtasks);
        for (i = 0; i < NRA; i++)
            for (j = 0; j < NCA; j++)
                a[i][j] = 10;
        for (i = 0; i < NCA; i++)
            for (j = 0; j < NCB; j++)
                b[i][j] = 10;
        printf("\n");

        // Prepare multiple data to send asynchronously to worker tasks
        requests = malloc(numworkers * maxRequestsCountPerCommunicator * sizeof(MPI_Request));
        prows = malloc(numworkers * sizeof(int));
        offsets = malloc(numworkers * sizeof(int));
        averow = NRA / numworkers;
        extra = NRA % numworkers;
        offsets[0] = 0;
        for (i = 0; i < numworkers - 1; i++)
        {
            prows[i] = (i < extra) ? averow + 1 : averow;
            offsets[i + 1] = offsets[i] + prows[i];
        }
        prows[numworkers - 1] = averow;

        // Send data in non blocking manner
        for (i = 0; i < numworkers; i++)
        {
            dest = i + 1;
            base = i * 4;
            rows = prows[i];
            offset = offsets[i];
            printf("Start incomplete sending %d rows to task %d offset= %d\n",
                   rows, dest, offset);
            MPI_Isend(&rows, 1, MPI_INT, dest,
                      FROM_MASTER, MPI_COMM_WORLD, requests + base + 0);
            MPI_Isend(&a[offset][0], rows * NCA, MPI_DOUBLE, dest,
                      FROM_MASTER, MPI_COMM_WORLD, requests + base + 1);
            MPI_Isend(&b, NCA * NCB, MPI_DOUBLE, dest,
                      FROM_MASTER, MPI_COMM_WORLD, requests + base + 2);
        }
        for (dest = 1; dest <= numworkers; dest++)
        {
            printf("Wait upon sending completion for task %d\n", dest);
            base = (dest - 1) * 4;
            MPI_Wait(requests + base + 0, &status);
            MPI_Wait(requests + base + 1, &status);
            MPI_Wait(requests + base + 2, &status);
        }
        printf("\n");

        /* Receive results from worker tasks in non blocking manner*/
        for (i = 0; i < numworkers; i++)
        {
            source = i + 1;
            base = i;
            offset = offsets[i];
            rows = prows[i];
            MPI_Irecv(&c[offset][0], rows * NCB, MPI_DOUBLE,
                      source, FROM_WORKER,
                      MPI_COMM_WORLD, requests + base + 0);
            printf("Start receiving results from task %d\n", source);
        }
        for (source = 1; source <= numworkers; source++)
        {
            printf("Wait upon receive completion for task %d\n", source);
            base = source - 1;
            MPI_Wait(requests + base + 0, &status);
        }
        free(requests);
        free(prows);
        free(offsets);
        /* Print results */
        printf("****\n");
        printf("Result Matrix:\n");
        for (i = 0; i < NRA; i++)
        {
            printf("\n");
            for (j = 0; j < NCB; j++)
                printf("%6.2f ", c[i][j]);
        }
        printf("\n********\n");
        printf("Done.\n");
    }
    /******** worker task *****************/
    else
    { /* if (taskid > MASTER) */
        MPI_Recv(&rows, 1, MPI_INT, MASTER, FROM_MASTER,
                 MPI_COMM_WORLD, &status);
        MPI_Recv(&a, rows * NCA, MPI_DOUBLE, MASTER, FROM_MASTER,
                 MPI_COMM_WORLD, &status);
        MPI_Recv(&b, NCA * NCB, MPI_DOUBLE, MASTER, FROM_MASTER,
                 MPI_COMM_WORLD, &status);
        for (k = 0; k < NCB; k++)
            for (i = 0; i < rows; i++)
            {
                c[i][k] = 0.0;
                for (j = 0; j < NCA; j++)
                    c[i][k] = c[i][k] + a[i][j] * b[j][k];
            }
        MPI_Send(&c, rows * NCB, MPI_DOUBLE, MASTER,
                 FROM_WORKER, MPI_COMM_WORLD);
    }
    MPI_Finalize();
}