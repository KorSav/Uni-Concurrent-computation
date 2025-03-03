using StudentsRegister;

const int TEACHER_COUNT = 4;
Register register = new(3, 30, TEACHER_COUNT);
Teacher[] teachers = new Teacher[TEACHER_COUNT];
Thread[] threads = new Thread[TEACHER_COUNT];
for (int i = 0; i < TEACHER_COUNT; i++) {
    teachers[i] = new(register, i);
    threads[i] = new(teachers[i].ScoreAllStudents);
}
foreach (var th in threads)
    th.Start();

foreach (var th in threads)
    th.Join();

StudentPosition sp = new(register);
do {
    System.Console.Write($"G-{sp.IGroup + 1} S-{sp.IStudent + 1, -2}: ");
    for (int ic = 0; ic < TEACHER_COUNT; ic++)
        System.Console.Write($"{register[sp, ic],3} ");
    System.Console.WriteLine($"| {register.GetTotal(sp)}");
} while (sp.Next());
