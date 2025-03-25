package app;

import app.SeqImpl.SequentialMul;
import app.ThreadsImpl.StripeMul;
import app.Utils.MatrixComparator;
import app.Utils.MatrixPair;
import app.Utils.MatrixUtils;
import app.Utils.Result;

public class Main {
    public static void main(String[] args) {
        MatrixPair mp = MatrixUtils.generateRandomMatrices(500);
        Result resStripe = StripeMul.multiply(mp.getMatrixA(), mp.getMatrixB(), 12);
        Result resSeq = SequentialMul.multiply(mp.getMatrixA(), mp.getMatrixB());
        double diff = MatrixComparator.calcMaxAbsDiff(
                resStripe.getResultMatrix(),
                resSeq.getResultMatrix());
        System.out.println("Stripe result diff from sequential: %e".formatted(diff));
        System.out.println("Hello world!");
    }
}