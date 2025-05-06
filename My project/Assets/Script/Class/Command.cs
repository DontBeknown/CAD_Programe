using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class DrawLineCommand : ICommand
{
    private Line line;
    private List<Shape> shapes;

    public DrawLineCommand(Line line, List<Shape> shapes)
    {
        this.line = line;
        this.shapes = shapes;
    }

    public void Execute()
    {
        shapes.Add(line);  
        line.Draw();  
    }

    public void Undo()
    {
        shapes.Remove(line);  
    }
}

public class UndoRedoManager : MonoBehaviour
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();  
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}