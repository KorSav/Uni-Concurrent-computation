namespace StudentsRegister;

class StudentPosition
{
    public int IGroup { get; private set; }
    public int IStudent { get; private set; }

    private readonly int _groupCount;
    private readonly int _studCount;

    public StudentPosition(int iGroup, int iStudent, int groupCount, int studCount)
    {
        if (iGroup < 0 || iGroup >= groupCount)
            throw new ArgumentOutOfRangeException(nameof(iGroup));
        if (iStudent < 0 || iStudent >= studCount)
            throw new ArgumentOutOfRangeException(nameof(iStudent));
        IGroup = iGroup;
        IStudent = iStudent;
        _groupCount = groupCount;
        _studCount = studCount;
    }

    public StudentPosition(int iGroup, int iStudent, Register register)
        : this(iGroup, iStudent, register.GroupCount, register.StudCount)
    { }

    public StudentPosition(int groupCount, int studCount)
        : this(0, 0, groupCount, studCount)
    { }

    public StudentPosition(Register register)
        : this(0, 0, register.GroupCount, register.StudCount)
    { }

    public bool Next()
    {
        int nextStud = IStudent + 1;
        int groupInc = nextStud / _studCount;
        if (groupInc != 0 && IGroup + groupInc >= _groupCount)
            return false;

        IGroup += groupInc;
        IStudent = nextStud % _studCount;
        return true;
    }
}
