#include "./mpi_mm.h"

#define MASTER 0

int sizes[] = {500, 1000, 1500, 2000, 3000};
int num_sizes = sizeof(sizes) / sizeof(sizes[0]);

int main(int argc, char *argv[])
{
    int numtasks, taskid, s, n;
    double *a, *b, *c;
    FILE *fp;
    const char *file_path;
    double start, end, time_blocking, time_nonblocking, speedup;

    if (argc != 3)
    {
        fprintf(stderr, "Usage: %s <output_file_path> <separator>\n", argv[0]);
        return 1;
    }

    MPI_Init(&argc, &argv);
    MPI_Comm_size(MPI_COMM_WORLD, &numtasks);
    MPI_Comm_rank(MPI_COMM_WORLD, &taskid);

    if (taskid == MASTER)
    {
        fp = NULL;
        file_path = argv[1];
        fp = fopen(file_path, "a");
        if (!fp)
        {
            perror("fopen");
            MPI_Abort(MPI_COMM_WORLD, 1);
        }
    }

    for (s = 0; s < num_sizes; ++s)
    {
        n = sizes[s];
        if (taskid == MASTER)
        {
            a = malloc(n * n * sizeof(double));
            b = malloc(n * n * sizeof(double));
            c = malloc(n * n * sizeof(double));
            if (!a || !b || !c)
            {
                fprintf(stderr, "Memory allocation failed\n");
                MPI_Abort(MPI_COMM_WORLD, 1);
            }
            fill_matrix(a, 1, n, n);
            fill_matrix(b, 1, n, n);
            printf("Benchmarking %d x %d matrices\n", n, n);
        }

        // Blocking
        MPI_Barrier(MPI_COMM_WORLD);
        if (taskid == MASTER)
            start = MPI_Wtime();
        mpi_mm(a, b, c, n, n, n, 0);
        if (taskid == MASTER)
        {
            end = MPI_Wtime();
            time_blocking = end - start;
        }

        // Non-blocking
        MPI_Barrier(MPI_COMM_WORLD);
        if (taskid == MASTER)
            start = MPI_Wtime();
        mpi_mm_nb(a, b, c, n, n, n, 0);
        if (taskid == MASTER)
        {
            end = MPI_Wtime();
            time_nonblocking = end - start;

            speedup = time_blocking / time_nonblocking;

            fprintf(fp, "%.3f%4$s%.3f%4$s%.3f%4$s",
                    time_blocking, time_nonblocking, speedup,
                    argv[2]);
            fflush(fp);
            free(a);
            free(b);
            free(c);
        }
    }

    if (taskid == MASTER)
        fclose(fp);
    MPI_Finalize();
    return 0;
}