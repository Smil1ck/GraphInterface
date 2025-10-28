// GraphErmakov/Commands/ICommand.cs
namespace GraphErmakov.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}