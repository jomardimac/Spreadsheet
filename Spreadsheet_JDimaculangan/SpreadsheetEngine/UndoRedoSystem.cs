//Name: Jomar Dimaculangan
//ID: 11422439

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace CptS321 {
    //notes and such was from Evan Old's lectures
    //undoredosystem requries:
    //interface that has an execute passed the sheet in
    //restore text and restore bgcolor
    //array/tupple of the interfaces, if we iterate through it in order, it wil lgo reverse and has the redo
    public interface IInvertibleCmd {
        //a universal interface that can do both undo and redo in execute():
        IInvertibleCmd Exec ();

        //this will be for redo idk why it wont work with just one exec:
        IInvertibleCmd Exec (string newText, int bgcolll);
        //same with ints:
    }

    public class RestoreText : IInvertibleCmd {
        //gonna just use a cell and text for restoring text:
        private string _text;
        private AbsCell _cell;

        //constructor:
        public RestoreText (string newT, AbsCell cell) {
            _text = newT;
            _cell = cell;
        }

        //returns he inverse a calls restore text to restore the old text:
        public IInvertibleCmd Exec () {
            var inverse = new RestoreText(_text, _cell);
            _cell.Text = _text;
            return inverse;
        }

        //same as above but with a set text:
        public IInvertibleCmd Exec (string newText, int bgcolll) {
            var inverse = new RestoreText(newText, _cell);
            _cell.Text = newText;
            return inverse;

        }
    }

    public class RestoreBGColor : IInvertibleCmd {
        private int _bgcol;
        private AbsCell _cell;

        //constructor:
        public RestoreBGColor (int newcol, AbsCell cell) {
            _bgcol = newcol;
            _cell = cell;
        }

        //similar to restore text but with uint bgcol
        public IInvertibleCmd Exec () {
            var inverse = new RestoreBGColor(_bgcol, _cell);
            _cell.BGColor = _bgcol;
            return inverse;
        }

        public IInvertibleCmd Exec (string newText, int bgcolll) {
            var inverse = new RestoreBGColor(_bgcol, _cell);
            _cell.BGColor = bgcolll;
            return inverse;
        }
    }
    //multiple commands:
    public class MultiCmd : IInvertibleCmd {
        //list of comands
        private List<IInvertibleCmd> _cmds = new List<IInvertibleCmd>();
        public string Text;
        public int backcol;

        //constructor:
        public MultiCmd (string newT, List<IInvertibleCmd> cmds) {
            Text = newT;
            _cmds = cmds;
        }
        public MultiCmd (string newT, List<IInvertibleCmd> cmds, int colo) {
            Text = newT;
            _cmds = cmds;
            backcol = colo;
        }
        //does multiple execs within one list
        public IInvertibleCmd Exec () {
            List<IInvertibleCmd> inv = new List<IInvertibleCmd>();
            //took me forever to figure this one out, go through set cmd and add each one as execing:
            foreach (IInvertibleCmd comd in _cmds) {
                inv.Add(comd.Exec());
            }
            return (new MultiCmd(this.Text, inv));
        }

        //same as above:
        public IInvertibleCmd Exec (string newText, int bgcol) {
            List<IInvertibleCmd> inv = new List<IInvertibleCmd>();
            foreach (IInvertibleCmd comd in _cmds) {
                inv.Add(comd.Exec(newText, bgcol));
            }
            return (new MultiCmd(this.Text, inv));
        }
    }

    //actually do undo and redo:
    public class UndoRedoSystem {
        //stack of multicomands will be save in an undo redo stack:
        private Stack<MultiCmd> _undo = new Stack<MultiCmd>();
        private Stack<MultiCmd> _redo = new Stack<MultiCmd>();

        //calls the previous one and pushes it to redo:
        public void undo () {
            MultiCmd cmd = _undo.Pop();
            cmd.Exec();
            _redo.Push(cmd);
        }
        //same as above but pushes in undo
        public void redo () {
            MultiCmd cmd = _redo.Pop();
            cmd.Exec(cmd.Text, cmd.backcol);
            _undo.Push(cmd);
        }
        //checker:
        public bool canUndo () { return _undo.Count > 0; }
        public bool canRedo () { return _redo.Count > 0; }

        //push to the undo stack:
        public void AddUndo (MultiCmd pushedCmd) {

            _undo.Push(pushedCmd);
        }

        //grab the text from undo stack :
        public string UndoText () {
            //can only do this if has something
            return _undo.Count > 0 ? _undo.Peek().Text : "";
        }

        //grab text from redo stack:
        public string RedoText () {
            //return top of stack string:
            return _redo.Count > 0 ? _redo.Peek().Text : "";
        }

        public void clearRedo () {
            _redo.Clear();
        }

        public void clearUndo () {
            _undo.Clear();
        }
    }

}