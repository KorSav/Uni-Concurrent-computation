#include <stdio.h>
#include <stdlib.h>
#include <time.h>

void fill(double *, double, int);

int main(int argc, char *argv[])
{
    int i, j, k, size;
    clock_t t0, t1;
    char *endptr;
    double elapsed;

    if (argc != 2)
    {
        fprintf(stderr, "Usage: %s <size of matrix>", argv[0]);
        return -1;
    }

    size = strtol(argv[1], &endptr, 10);
    if (*endptr != '\0')
    {
        perror("Invalid matrix size");
        return -1;
    }

    double (*a)[size], (*b)[size], (*c)[size];

    a = malloc(size * size * sizeof(double));
    b = malloc(size * size * sizeof(double));
    c = malloc(size * size * sizeof(double));
    fill(a, 10, size * size);
    fill(b, 10, size * size);
    t0 = clock();
    for (k = 0; k < size; k++)
        for (i = 0; i < size; i++)
        {
            c[i][k] = 0.0;
            for (j = 0; j < size; j++)
                c[i][k] = c[i][k] + a[i][j] * b[j][k];
        }
    t1 = clock();
    elapsed = (double)(t1 - t0) / CLOCKS_PER_SEC;
    printf("%.3f", elapsed);
    fflush(stdout);
    free(a);
    free(b);
    free(c);
    return 0;
}

void fill(double *m, double val, int size)
{
    int i;
    for (i = 0; i < size; i++)
        m[i] = val;
}