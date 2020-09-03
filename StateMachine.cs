using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace State_Machine
{
    public enum ProcessState //Состояние процесса
    { 
        q0, q1, q2, q3, q4, q5, q6, q7, q8, q9, q10, q11, q12, q13, q14
    }

    public enum ActionState //Состояние для задания имени
    {
        none, //
        n0, //Начало задания имени
        n1, //Продолжение задания
        n2, //Конец 
    }

    public enum Command
    {
        Space,
        Type_Without_System_and_Reference,
        Type_Without_System_and_No_Reference,
        System,
        Point,
        Type_with_System_and_Reference,
        Type_with_System_and_No_Reference,
        OpeningBracket, //Открывающая скобка [
        Question,
        Comma, //Запятая
        ClosingBracket, //Закрывающая скобка ]
        Lowercase_Symbols, //Строчные символы
        Numbers,
        Semicolon, // Точка с запятой
        Dog
    }

    public class StateMachine
    {
        Dictionary<Command, List<string>> Alphabet;

        List<ProcessState> F = new List<ProcessState>() { ProcessState.q0 }; //Множество конечных состояний

        List<string> Names;
        string buffer = "";
        public ActionState CurrentStateName { get; private set; }
        List<ActionState> HaltAction = new List<ActionState>() { ActionState.none, ActionState.n2 };

        class StateTransition
        {
            ProcessState CurrentState;
            Command Command;

            public StateTransition(ProcessState currentState, Command command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return CurrentState.GetHashCode() + Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                if (other == null)
                    return false;
                return this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        int index, startIndex, line, position;
        readonly string str;
        Dictionary<StateTransition, Tuple<ProcessState, ActionState>> transitions;
        public ProcessState CurrentState { get; private set; }


        public StateMachine(string str)
        {
            position = 0;
            line = 1;
            index = 0;
            this.str = str;
            CurrentState = ProcessState.q0;
            transitions = new Dictionary<StateTransition, Tuple<ProcessState, ActionState>>
            {
                { new StateTransition(ProcessState.q0, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q0, ActionState.none) },
                { new StateTransition(ProcessState.q0, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q1, ActionState.none) },
                { new StateTransition(ProcessState.q0, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q2, ActionState.none) },
                { new StateTransition(ProcessState.q0, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q3, ActionState.none) },
                { new StateTransition(ProcessState.q0, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q2, ActionState.none) },
                { new StateTransition(ProcessState.q0, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q1, ActionState.none) },

                { new StateTransition(ProcessState.q1, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q4, ActionState.none) }, 
                { new StateTransition(ProcessState.q1, Command.OpeningBracket), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q1, Command.Dog), new Tuple<ProcessState, ActionState>(ProcessState.q13, ActionState.none) },

                { new StateTransition(ProcessState.q2, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q5, ActionState.none) }, 
                { new StateTransition(ProcessState.q2, Command.OpeningBracket), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q2, Command.Question), new Tuple<ProcessState, ActionState>(ProcessState.q7, ActionState.none) }, 
                { new StateTransition(ProcessState.q2, Command.Dog), new Tuple<ProcessState, ActionState>(ProcessState.q13, ActionState.none) }, 

                { new StateTransition(ProcessState.q3, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q3, ActionState.none) },
                { new StateTransition(ProcessState.q3, Command.Point), new Tuple<ProcessState, ActionState>(ProcessState.q6, ActionState.none) },

                { new StateTransition(ProcessState.q4, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q4, ActionState.none) },
                { new StateTransition(ProcessState.q4, Command.OpeningBracket), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q4, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q4, Command.Dog), new Tuple<ProcessState, ActionState>(ProcessState.q13, ActionState.none) },
                { new StateTransition(ProcessState.q4, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q4, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q4, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q4, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q4, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q5, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q5, ActionState.none) },
                { new StateTransition(ProcessState.q5, Command.OpeningBracket),new Tuple<ProcessState, ActionState>( ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q5, Command.Question), new Tuple<ProcessState, ActionState>(ProcessState.q7, ActionState.none) },
                { new StateTransition(ProcessState.q5, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q5, Command.Dog), new Tuple<ProcessState, ActionState>(ProcessState.q13, ActionState.none) },
                { new StateTransition(ProcessState.q5, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q5, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q5, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q5, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q5, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q6, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q6, ActionState.none) },
                { new StateTransition(ProcessState.q6, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q2, ActionState.none) },
                { new StateTransition(ProcessState.q6, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q1, ActionState.none) },

                { new StateTransition(ProcessState.q7, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q7, ActionState.none) },
                { new StateTransition(ProcessState.q7, Command.OpeningBracket), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q7, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q7, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },//
                { new StateTransition(ProcessState.q7, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q7, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q7, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q7, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q8, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q8, Command.Comma), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q8, Command.ClosingBracket), new Tuple<ProcessState, ActionState>(ProcessState.q9, ActionState.none) },

                { new StateTransition(ProcessState.q9, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q9, ActionState.none) },
                { new StateTransition(ProcessState.q9, Command.OpeningBracket), new Tuple<ProcessState, ActionState>(ProcessState.q8, ActionState.none) },
                { new StateTransition(ProcessState.q9, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q9, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q9, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q9, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q9, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q9, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q10, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q12, ActionState.n2) },
                { new StateTransition(ProcessState.q10, Command.Comma), new Tuple<ProcessState, ActionState>(ProcessState.q11, ActionState.n2) },
                { new StateTransition(ProcessState.q10, Command.Numbers), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Semicolon), new Tuple<ProcessState, ActionState>(ProcessState.q0, ActionState.n2) },//n2
                { new StateTransition(ProcessState.q10, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q10, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },


                { new StateTransition(ProcessState.q11, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q11, ActionState.none) },
                { new StateTransition(ProcessState.q11, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q11, Command.Dog), new Tuple<ProcessState, ActionState>(ProcessState.q13, ActionState.none) },
                { new StateTransition(ProcessState.q11, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q11, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q11, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q14, ActionState.n0) },
                { new StateTransition(ProcessState.q11, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q11, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q12, Command.Space), new Tuple<ProcessState, ActionState>(ProcessState.q12, ActionState.none) },
                { new StateTransition(ProcessState.q12, Command.Comma), new Tuple<ProcessState, ActionState>(ProcessState.q11, ActionState.none) },
                { new StateTransition(ProcessState.q12, Command.Semicolon), new Tuple<ProcessState, ActionState>(ProcessState.q0, ActionState.none) },

                { new StateTransition(ProcessState.q13, Command.Lowercase_Symbols), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q13, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q13, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q13, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q13, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },
                { new StateTransition(ProcessState.q13, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n0) },

                { new StateTransition(ProcessState.q14, Command.System), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q14, Command.Type_Without_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q14, Command.Type_Without_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q14, Command.Type_with_System_and_No_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q14, Command.Type_with_System_and_Reference), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                { new StateTransition(ProcessState.q14, Command.Numbers), new Tuple<ProcessState, ActionState>(ProcessState.q10, ActionState.n1) },
                //{ new StateTransition(ProcessState.q14, Command.Semicolon), new Tuple<ProcessState, ActionState>(ProcessState.q0, ActionState.n2) },//n2
            };

            Alphabet = new Dictionary<Command, List<string>>
            {
                { Command.Space, new List<string> {" ", "\n", "\t", "\r"}},
                { Command.Lowercase_Symbols, new List<string> {"_", "a-z", "A-Z"}},
                { Command.Numbers, new List<string> {"0-9"}},
                { Command.Type_Without_System_and_Reference, new List<string> {"string", "object"} },
                { Command.Type_Without_System_and_No_Reference, new List<string> { "int", "byte", "short", "long", "char", "float", "double", "decimal" } },
                { Command.System, new List<string> {"System"}},
                { Command.Type_with_System_and_No_Reference, new List<string> { "Int16", "Int32", "Int64", "Byte", "Decimal", "Boolean", "Char", "Double", "Single" }},
                { Command.Type_with_System_and_Reference, new List<string> { "Object", "String" }},
                { Command.ClosingBracket, new List<string> { "]" }},
                { Command.OpeningBracket, new List<string> { "[" }},
                { Command.Comma, new List<string> { "," }},
                { Command.Point, new List<string> { "." }},
                { Command.Question, new List<string> { "?" }},
                { Command.Semicolon, new List<string> { ";" }},
                { Command.Dog, new List<string> { "@" }}
            };

            Names = new List<string>() {"" };
        }

        public void Go()
        {
            int i = 0;
            while(i != str.Length)
            {
                if (str[i] == '\n') { line++; position = 0; }
                Command? cmd = null;
                foreach (KeyValuePair<Command, List<string>> alphabet in Alphabet)
                {
                    foreach (string value in alphabet.Value)
                    {
                        if (value.Length == 1)
                        {
                            if (str[i].ToString() == value)
                                cmd = alphabet.Key;
                        }
                        else if (value.Length == 3 && value[1] == '-')
                        {
                            if(str[i] >= value[0] && str[i] <= value[2])
                            {
                                index = i;
                                cmd = alphabet.Key;
                            }
                        }
                        else if (str.IndexOf(value, i) == i) 
                        {
                            startIndex = i;
                            i = i + value.Length - 1;
                            index = i; position += value.Length - 1;
                            cmd = alphabet.Key;
                        }
                    }
                }

                if (cmd == null)
                    Exit($"ERROR in lines {line} in position: {position}");
                else
                    MoveNext((Command)cmd);
                Console.WriteLine("Current State = " + CurrentState);
                Console.WriteLine("Current ActionName = " + CurrentStateName);
                position++;
                i++;
            }
            IsHALT();
        }

        void IsHALT()
        {
            int temp = F.IndexOf(CurrentState);
            if (DoAction(HaltAction[temp]))
                Exit("HALT");
            else
                Exit($"ERROR in lines {line} in position: {position}");
        }
        
        private void Exit(string _out)
        {
            Console.WriteLine(_out);
            Console.Read();
            Environment.Exit(0);
        }

        private bool DoAction(ActionState action)
        {
            if (action == ActionState.n2)
            {
                buffer = buffer.Remove(0, 1);
                if (Names.Contains(buffer))
                    return false;
                else
                {
                    Names[Names.Count - 1] = buffer;
                    buffer = "";
                    Names.Add("");
                }
            }
            else if(action == ActionState.n0 || action == ActionState.n1) 
            {
                for(int i = startIndex; i <= index; i++)
                    buffer += str[i];
            }
            return true;
        }

        private ProcessState GetNext(Command command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);

            Tuple<ProcessState, ActionState> tuple;
            ProcessState nextState;

            if (!transitions.TryGetValue(transition, out tuple))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command + $"  Exception in lines {line} in position: {position}");

            nextState = tuple.Item1;
            CurrentStateName = tuple.Item2;

            if (!DoAction(CurrentStateName))
                Exit($"ERROR in lines {line} in position: {position}");

            return nextState;
        } 

        private ProcessState MoveNext(Command command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }
    }
}


// foreach (Command c in Alphabet.Keys) { Alphabet[c] }
//public class All_Type
//{
//    List<string> _Type_Without_System_and_Reference;
//    List<string> _Type_Without_System_and_No_Reference;
//    List<string> _Type_with_System_and_No_Reference;
//    List<string> _Type_with_System_and_Reference;
//    List<string> _Name;
//    readonly string _system;

//    public string System { get { return _system; } }

//    public List<string> Type_Without_System_and_Reference
//    {
//        get { return _Type_Without_System_and_Reference; }
//    }

//    public void Add_Type_Without_System_and_Reference(string str)
//    {
//        _Type_Without_System_and_Reference.Add(str);
//    }

//    public List<string> Type_Without_System_and_No_Reference
//    {
//        get { return _Type_Without_System_and_No_Reference; }
//    }

//    public List<string> Type_with_System_and_No_Reference
//    {
//        get { return _Type_with_System_and_No_Reference; }
//    }

//    public List<string> Type_with_System_and_Reference
//    {
//        get { return _Type_with_System_and_Reference; }
//    }

//    public List<string> Name
//    {
//        get { return _Name; }
//    }

//    public void Add_Name(string str)
//    {
//        _Name.Add(str);
//    }

//    public All_Type()
//    {
//        _Type_Without_System_and_Reference = new List<string> {"string", "object"};
//        _Type_Without_System_and_No_Reference = new List<string> { "int", "byte", "short", "long", "char", "float", "double", "decimal" };
//        _system = "System";
//        _Type_with_System_and_No_Reference = new List<string> { "Int16", "Int32", "Int64", "Byte", "Decimal", "Boolean", "Char", "Double", "Single" };
//        _Type_with_System_and_Reference = new List<string> { "Object", "String" };
//        _Name = new List<string> {"" };
//    }
//}

//bool Method__(ref int i, string value)
//{

//}

//All_Type All_Type = new All_Type();

//public void Definition_Command() // Определение команд
//{
//    int size_text = str.Length;
//    string temp = String.Empty;
//    bool type = false, system = false;
//    for (int i = 0; i < size_text; i++)
//    {
//        if (str[i] == ' ')
//        {
//            MoveNext(Command.Space);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if ((str[i] >= 97) && (str[i] <= 122) && (type == false))
//        {
//            temp = "";
//            bool ok = false;
//            while (str[i] >= 97 && str[i] <= 122)
//            {
//                temp += str[i];
//                i++;
//            }


//            foreach(var check_Type in All_Type.Type_Without_System_and_No_Reference)
//            {
//                if(temp == check_Type)
//                {
//                    MoveNext(Command.Type_Without_System_and_No_Reference);
//                    Console.WriteLine("Current State = " + CurrentState);
//                    type = true; ok = true;
//                    break;
//                }                       
//            }

//            foreach (var check_Type in All_Type.Type_Without_System_and_Reference)
//            {
//                if (temp == check_Type)
//                {
//                    MoveNext(Command.Type_Without_System_and_Reference);
//                    Console.WriteLine("Current State = " + CurrentState);
//                    type = true; ok = true;
//                    break;
//                }
//            }

//            if (ok != true)
//                Exit("Неверный тип!");
//        }

//        if((str[i] >= 65) && (str[i] <= 90) && (type == false) && (system == false))
//        {
//            temp = "";
//            temp += str[i];
//            i++;
//            while ((str[i] >= 97 && str[i] <= 122)) 
//            {
//                temp += str[i];
//                i++;
//            }
//            if (temp == All_Type.System)
//            {
//                MoveNext(Command.System);
//                Console.WriteLine("Current State = " + CurrentState);
//                system = true;
//            }
//        }

//        if (str[i] == '.')
//        {
//            MoveNext(Command.Point);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if ((str[i] >= 65) && (str[i] <= 90) && (system == true))
//        {
//            temp = "";
//            bool ok = false;
//            temp += str[i];
//            i++;
//            while ((str[i] >= 97 && str[i] <= 122) || (str[i] >= 48 && str[i] <= 57))
//            {
//                temp += str[i];
//                i++;
//            }
//            foreach (var check_Type in All_Type.Type_with_System_and_Reference)
//            {
//                if (temp == check_Type)
//                {
//                    MoveNext(Command.Type_with_System_and_Reference);
//                    Console.WriteLine("Current State = " + CurrentState);
//                    type = true; ok = true;
//                    break;
//                }
//            }

//            foreach (var check_Type in All_Type.Type_with_System_and_No_Reference)
//            {
//                if (temp == check_Type)
//                {
//                    MoveNext(Command.Type_with_System_and_No_Reference);
//                    Console.WriteLine("Current State = " + CurrentState);
//                    type = true; ok = true;
//                    break;
//                }
//            }

//            if (ok != true)
//                Exit("Неверный тип!");
//        }

//        if (str[i] == '[')
//        {
//            MoveNext(Command.OpeningBracket);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if (str[i] == '?')
//        {
//            MoveNext(Command.Question);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if (str[i] == ',')
//        {
//            MoveNext(Command.Comma);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if (str[i] == ']')
//        {
//            MoveNext(Command.ClosingBracket);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if ((str[i] >= 97) && (str[i] <= 122) && (type == true))
//        {
//            temp = "";
//            temp += str[i];
//            MoveNext(Command.Lowercase_Symbols);
//            Console.WriteLine("Current State = " + CurrentState);
//        }

//        if (((str[i] >= 97 && str[i] <= 122) || (str[i] >= 48 && str[i] <= 57) || (str[i] == 95)) && (type == true))
//        {
//            temp = "";
//            temp += str[i];
//            while ((str[i] >= 97 && str[i] <= 122) || (str[i] >= 48 && str[i] <= 57) || (str[i] == 95))
//            {
//                temp += str[i];
//                i++;
//            }

//            foreach (var name in All_Type.Name)
//            {
//                if (All_Type.Name.Contains(temp))//
//                {
//                    Exit($"Локальная переменная с именем {temp} уже определена");
//                }
//                else
//                {
//                    MoveNext(Command.Numbers_and_Symbols);
//                    Console.WriteLine("Current State = " + CurrentState);
//                    All_Type.Name.Add(temp);
//                    break;
//                }
//            }
//        }

//        if (str[i] == ';')
//        {
//            MoveNext(Command.Semicolon);
//            Console.WriteLine("Current State = " + CurrentState);
//            type = false; system = false;
//        }

//    }

//    if((CurrentState != ProcessState.q0))
//    {
//        Exit("Конец строки, но объявление не закончено");
//    }

/*
 *         List<string> Name = new List<string>() { "" };
 *             string temp_name = "";
 *                                             temp_name += str[i];
 * if(cmd != Command.Lowercase_Symbols && cmd != Command.Numbers && temp_name != "")
            {
                foreach (var name in Name)
                {
                    if(Name.Contains(temp_name))
                        Exit($"Локальная переменная с именем {temp_name} уже определена");
                    else
                    {
                        Name.Add(temp_name);
                        temp_name = "";
                        break;
                    }
                }
            }*/
