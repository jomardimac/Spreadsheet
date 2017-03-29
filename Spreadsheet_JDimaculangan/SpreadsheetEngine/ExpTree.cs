//Jomar Dimaculangan
//11422439
//THIS WAS REFERENCED BY EVAN OLDS's LECTURES.
//HE BASICALLY WENT THROUGH THE DATA STRUCTURE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace CptS321 {

    //some/alout are implemented by Evan Olds during Lecture

    public class ExpTree {
        //has one member: an abstract node that is m_root;
        private Node m_root;

        //Create a dictionary for variable names:
        public static Dictionary<string, double> m_lookup = new Dictionary<string, double>();
        // Constructor:
        public ExpTree (string exp) {
            //TODO :Parse the expression string and build the tree:
            //for next homework: support:
            //no parens, single operator:
            //"A1+47+654+Hello+2
            //54+275+98
            //6*7*8
            //"A2"
            //FOR HW6:
            //"2+3*4"
            //2*3+4
            // 2*((3+4)-7)
            //"(55-11) / 11"
            //(2*3)+4
            //((2+3)*4)-(1+3)
            m_lookup.Clear();
            //handle whitespace:
            exp = exp.Replace(" ", "");
            //runs compile to the expression which creates the tree.
            this.m_root = Compile(exp);
        }

        public void SetVar (string varName, double varValue) {
            if (m_lookup.ContainsKey(varName)) {
                m_lookup.Remove(varName);
            }
            m_lookup.Add(varName, varValue);
        }

        //grab each variables:
        public List<string> grabVars() {
            List<string> listofstrings = new List<string>(m_lookup.Keys);
            return listofstrings;
        }

        //let Node evaluate variable names, constant nodes and operator nodes:
        //in order to evaluate those, make this an abstract class
        private abstract class Node {
            
            public abstract double Eval ();
        }

        //Create a VarNode class if the input given is a variable type:
        //this should include an eval function that grabs the value using the key:
        private class VarNode : Node {
            private string m_varName;

            //constructor for the compile method:
            public VarNode (string str) {
                this.m_varName = str;
            }

            public override double Eval () {

                return m_lookup[m_varName];
            }
        }

        //Create a ConstNode if the value is double:
        //Should include eval function that grabs the double m_Value:
        private class ConstNode : Node {
            private double m_value;

            public ConstNode (double value) {
                m_value = value;
            }


            public override double Eval () {
                return m_value;
            }
        }

        //Create an OpNode when a char "+,-,/,or*" is given:
        //Evaluating expressions will be using this:
        private class OpNode : Node {
            private char m_op;
            private Node m_left, m_right;

            public OpNode (char op, Node childrenL, Node childrenR) {
                m_op = op;
                m_left = childrenL;
                m_right = childrenR;
            }
            //do left op right:
            public override double Eval () {
                //grab left and right node children and their double numbers:
                double left = m_left.Eval();
                double right = m_right.Eval();
                //evaluate their double values by doing left op right
                switch (m_op) {
                    case '+':
                        return left + right;
                    case '-':
                        return left - right;
                    case '*':
                        return left * right;
                    case '/':
                        return left / right;
                        //etc
                }
                return 0;
            }
        }

        //only building one node from a string:
        private static Node BuildSimple (string term) {
            double num;
            //tries to parse the string and returns true if the term is a number.
            if (double.TryParse(term, out num)) {
                return new ConstNode(num);
            }
            //if its not a string, add it in a dictionary node or varnode:
            
            //try this:
            else{
                
                  m_lookup[term]=0;
            }
            return new VarNode(term);
        }

        private static Node Compile (string exp) {
            //find first operator:
            //build parent operator node:
            //parent.left = buildsimple before op char
            //parent.right = compile(after opchar)
            //return parent;

            exp = exp.Replace(" ", "");
            //never find an operator:

            //TODO: SPECIAL CASE:
            //check for being entirely enclosed in ():
            // (3+4+5)
            // (3+4) * (5+6)
            //if first char is '(' and last char a matching )', remove parns:
            if (exp [0] == '(') {
                //counter for parenthesis is 1:
                int counter = 1;
                //go through the string until we find a closing parenthesis:
                for (int i = 1; i < exp.Length; i++) {
                    //found!
                    if (exp [i] == ')') {
                        //decrement the counter
                        counter--;
                        //only do take out the parenthesis if the whole expression is in (expression);
                        if (counter == 0) {
                            //if the ending is parenthesis, just delete that ')':
                            if (i == exp.Length - 1) {
                                //this is the hard part: take out '(' AND ')' and recursively call it 
                                return Compile(exp.Substring(1, exp.Length - 2));

                            }
                            //dont have the close at the very end
                            else {
                                break;
                            }
                        }
                    }
                    //if it isn't, add a counter: 
                    if (exp [i] == '(') {
                        counter++;

                    }
                }
            }


            //get low op index:
            //build opnode for char at that index:
            int index = GetLowOpIndex(exp);
            if (index != -1) {
                //found the lowest prioarty:
                return new OpNode(
                    exp [index],
                    Compile(exp.Substring(0, index)),
                    Compile(exp.Substring(index + 1)));
            }
            return BuildSimple(exp);

         }
        //ExpTree Eval that grabs the Node's eval:
        public double Eval () {
            if (m_root != null) {
                return m_root.Eval();
            }
            else {
                return double.NaN;
            }
        }

        //This will be for the next assignment:

        private static int GetLowOpIndex (string exp) {

            // 3+4*5 + 6
            // 3*4+5 + 6
            // 3-(4*(5-6))

            int parentCounter = 0;
            int index = -1;
            for (int i = exp.Length - 1; i >= 0; i--) {
                switch (exp [i]) {
                    case ')':
                        parentCounter--;
                        break;
                    case '(':
                        parentCounter++;
                        break;
                    case '+':
                    case '-':
                        if (parentCounter == 0) {
                            return i;
                        }
                        break;
                    case '*':
                    case '/':
                        if (parentCounter == 0 && index == -1) {
                            index = i;
                        }
                        break;
                }
            }
            return index;
        }

    }
}
