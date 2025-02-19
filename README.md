# Concurrent computation technologies
This is a series of practical tasks from my university discipline.

## Lab 1
### Thread balls project
Allows to create balls in separate threads each responsible for ball's movement.
1. In the [subtask](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.1/lab1/ThreadBalls/MainWindow.xaml.cs) it is possible to create new ball. Increase in their amount leads to lag in balls' movement.
2. In the [subtask](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.2/lab1/ThreadBalls/MainWindow.xaml.cs) pockets are added. When ball is potted, thread is terminated and number of potted balls is updated. PottedCount in [Pocket.cs](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.2/lab1/ThreadBalls/Pocket.cs) is fixed [later](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1/lab1/ThreadBalls/Pocket.cs).
3. In the [subtask](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.3/lab1/ThreadBalls/MainWindow.xaml.cs) 5 experiments on thread priority are taken (blue ball has lower priority). In the result, thread priority has significant effect when number of threads is big enough.
4. In the [subtask](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.4/lab1/ThreadBalls/MainWindow.xaml.cs) thread join is demonstated. ThreadRed call ThreadBlue.Join() => red ball moves only when blue one disappears.

### Counter project
First thread increments counter value, the other decrements. [Without synchronization](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.5_1/lab1/Counter/Program.cs) counter value in the end is wrong. To synchronize counter used 3 methods:
- [synchronized method](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.5_2~2/lab1/Counter/Program.cs)
- [synchronized block](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.5_2~1/lab1/Counter/Program.cs)
- [synchronized object](https://github.com/KorSav/Uni-Concurrent-computation/blob/lab-1.5_2/lab1/Counter/Program.cs)