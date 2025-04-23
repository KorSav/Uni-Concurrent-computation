#include "./mpi_mm.h"

#define NRA 62        /* number of rows in matrix A */
#define NCA 15        /* number of columns in matrix A */
#define NCB 7         /* number of columns in matrix B */
#define MASTER 0      /* taskid of first task */
#define FROM_MASTER 1 /* setting a message type */
#define FROM_WORKER 2 /* setting a message type */

int main(int argc, char *argv[])
{
    int numtasks, taskid, i, j;
    double a[NRA][NCA], /* matrix A to be multiplied */
        b[NCA][NCB],    /* matrix B to be multiplied */
        c[NRA][NCB];    /* result matrix C */

    MPI_Init(&argc, &argv);
    MPI_Comm_size(MPI_COMM_WORLD, &numtasks);
    MPI_Comm_rank(MPI_COMM_WORLD, &taskid);

    if (taskid == MASTER)
    {
        printf("mpi_mm has started with %d tasks.\n", numtasks);
        fill_matrix(a, 10, NRA, NCA);
        fill_matrix(b, 10, NCA, NCB);
        printf("\n");
    }

    mpi_mm_nb((double *)a, (double *)b, (double *)c, NRA, NCA, NCB, 1);

    if (taskid == MASTER)
    {
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
    MPI_Finalize();
}
