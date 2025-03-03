namespace StudentsRegister;

class Teacher(Register register, int cell)
{
    private readonly Register _register = register;
    private readonly int _cell = cell;
    private readonly Random _random = new();

    public void ScoreStudent(StudentPosition sp)
    {
        int total = _register.GetTotal(sp);
        if (total >= 100)
            return;
        int maxAllowedPoint = 100 - total;
        int point = _random.Next(maxAllowedPoint);
        _register[sp][_cell] = point;
    }

    public void ScoreStudentSync(StudentPosition sp)
    {
        lock (_register[sp]) {
            int total = _register.GetTotal(sp);
            if (total >= 100)
                return;
            int maxAllowedPoint = 100 - total;
            int point = _random.Next(maxAllowedPoint);
            _register[sp][_cell] = point;
        }
    }
}