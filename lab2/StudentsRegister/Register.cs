namespace StudentsRegister;

class Register
{
    private readonly int[,][] _points;
    public readonly int GroupCount;
    public readonly int StudCount;

    public Register(int groupCount, int studCount, int cellCount)
    {
        _points = new int[groupCount, studCount][];
        StudentPosition sp = new(groupCount, studCount);
        do
            this[sp] = new int[cellCount];
        while (sp.Next());

        GroupCount = groupCount;
        StudCount = studCount;
    }

    public int[] this[StudentPosition sp] {
        get => _points[sp.IGroup, sp.IStudent];
        init => _points[sp.IGroup, sp.IStudent] = value;
    }

    public int GetTotal(StudentPosition sp)
    {
        int total = 0;
        foreach (int point in _points[sp.IGroup, sp.IStudent])
            total += point;
        return total;
    }
}
