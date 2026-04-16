using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TFLC_sem6_lab1.Scanner;
using TFLC_sem6_lab1.AST;

namespace TFLC_sem6_lab1.Grammar
{

    public class Parser
    {
        private List<TableLine> _tokens;
        private int _currentPos;
        private TableLine _currentToken;
        private List<ParseError> _errors;
        private TokenDict _tokenDict;
        private int[] relation_operation = new int[] { 13, 14, 15, 16, 19, 20 };
        private int[] increment_operation = new int[] { 6, 8 };
        private bool _errorFlag;
        private int _errorCounter;
        private bool endFlag = false;
        public AstNode Root { get; private set; }
        private SymbolTable _symbolTable = new SymbolTable();

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _tokenDict = new TokenDict();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
            _errorFlag = false;
        }

        public void DisplayErrors(List<ParseError> errors, DataGridView SyntaxTable)
        {
            SyntaxTable.Rows.Clear();

            foreach (var error in errors)
            {
                string locationText = "";
                if (error.StartPosition >= 0 && error.EndPosition >= error.StartPosition)
                {
                    locationText = $"строка {error.Line + 1}, {error.StartPosition}-{error.EndPosition}";
                }
                else if (error.Line >= 0)
                {
                    locationText = $"строка {error.Line + 1}";
                }
                else
                {
                    locationText = "местоположение не определено";
                }

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(SyntaxTable,
                    error.Message,
                    locationText
                );

                SyntaxTable.Rows.Add(row);
            }
        }

        public List<ParseError> Parse()
        {
            try
            {
                Root = Program(); 
            }
            catch (Exception ex)
            {
                AddError($"Критическая ошибка: {ex.Message}", 0, 0, 0);
            }

            return _errors;
        }

        private void NextToken()
        {
            _currentPos++;
            if (_currentPos < _tokens.Count)
                _currentToken = _tokens[_currentPos];
            else
                _currentToken = null;
        }

        private void AddError(string message, int line, int start_position, int end_position)
        {
            _errors.Add(new ParseError
            {
                Message = message,
                Line = line,
                StartPosition = start_position,
                EndPosition = end_position
            });
        }

        private bool IsValidToken()
        {
            return _currentPos < _tokens.Count && _currentToken != null;
        }

        private bool SkipToToken(int code)
        {
            int pos = _currentPos;
            int i = 0;
            while (IsValidToken() && _currentToken.code != code && i < 3)
            {
                NextToken();
                if (_currentToken != null)
                {
                    if (_currentToken.code == code)
                    {
                        return true;
                    }
                    i++;
                }
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private bool SkipToToken(int[] codes)
        {
            int pos = _currentPos;
            int i = 0;
            while (IsValidToken() && !codes.Contains(_currentToken.code) && i < 3)
            {
                NextToken();
                if (_currentToken != null)
                {
                    if (codes.Contains(_currentToken.code)) return true;
                }
                i++;
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private void SkipToSynchronizingToken(int[] codes)
        {
            int pos = _currentPos;
            int i = 0;
            while (IsValidToken() && i < 3)
            {
                if (codes.Contains(_currentToken.code))
                {
                    return;
                }
                NextToken();
                i++;
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
        }

        public AstNode Program()
        {
            if (!IsValidToken()) return null;

            var node = DoWhileStatement();

            if (_errorCounter > 3) return node;

            if (IsValidToken())
            {
                AddError($"Ошибочный токен в конце строки",
                    _currentToken.line_number,
                    _currentToken.start_pos,
                    _currentToken.end_pos);
            }

            return node;
        }

        private DoWhileNode DoWhileStatement()
        {
            if (!IsValidToken()) return null;

            BlockNode blockNode = null;
            AstNode conditionNode = null;

            if (_currentToken.code != 3)
            {
                AddError("Ошибочный токен в начале строки",
                    _tokens[_currentPos].line_number,
                    _tokens[_currentPos].start_pos,
                    _tokens[_currentPos].end_pos);

                if (SkipToToken(3)) SkipToToken(3);
                else SkipToSynchronizingToken([3, 9, 1]);

                _errorCounter++;
            }

            if (!IsValidToken())
            {
                AddError("В конструкции do-while ожидается do",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            if (_currentToken.code == 3)
            {
                NextToken();
                _errorCounter = 0;
            }
            else
            {
                if (_errorCounter > 3)
                {
                    AddError("Допущено более 3 синтаксических ошибок, " +
                        "парсинг прекращается", _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                    endFlag = true;
                    return null;
                }
                if (_errorCounter == 0)
                {
                    AddError($"В конструкции do-while: ожидается ключевое слово do",
                        _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(3)) SkipToToken(3);
                else SkipToSynchronizingToken([3, 9, 1]); // do { $id
                _errorCounter++;
            }


            if (!IsValidToken() || _tokens.Count == 1)
            {
                AddError("В условном выражении: ожидается открывающая фигурная скобка",
                    _tokens[0].line_number,
                    _tokens[0].start_pos,
                    _tokens[0].end_pos);
                NextToken();
                return null;
            }

            blockNode = Block();
            if (endFlag) return null;

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается ключевое слово while",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                return null;
            }

            if (_currentToken.code == 2)
            {
                NextToken();
                _errorCounter = 0;
            }
            else
            {
                if (_errorCounter > 3)
                {
                    AddError("Допущено более 3 синтаксических ошибок, " +
                        "парсинг прекращается", _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                    endFlag = true;
                    return null;
                }
                if (_errorCounter == 0)
                {
                    AddError($"В конструкции do-while: ожидается ключевое слово while",
                        _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(2)) SkipToToken(2);
                else SkipToSynchronizingToken([2, 11, 1]); // while ( $id
                _errorCounter++;
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается (",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            conditionNode = Condition();
            if (endFlag) return null;

            if (IsValidToken() && _currentToken.code == 17)
            {
                NextToken();
                _errorCounter = 0;
            }
            else
            {
                if (_errorCounter > 3)
                {
                    AddError("Допущено более 3 синтаксических ошибок, " +
                        "парсинг прекращается", _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                    endFlag = true;
                    return null;
                }

                if (_errorCounter == 0)
                {
                    AddError($"Ожидается завершающая точка с запятой",
                   _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                }
                _errorCounter = 0;
            }

            return new DoWhileNode
            {
                Body = blockNode,
                Condition = conditionNode
            };
        }

        private AstNode Condition()
        {
            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается открывающая круглая скобка",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            if (_currentToken.code == 11)
            {
                NextToken();
                _errorCounter = 0;
            }

            AstNode left = null;

            if (_currentToken.code == 1)
            {
                string name = _currentToken.token;

                if (_symbolTable.Lookup(name) == null)
                {
                    AddError($"Переменная {name} не объявлена",
                        _currentToken.line_number,
                        _currentToken.start_pos,
                        _currentToken.end_pos);
                }

                left = new VariableNode { Name = name };
            }
            else if (_currentToken.code == 4)
            {
                left = new LiteralNode { Value = int.Parse(_currentToken.token) };
            }

            NextToken();

            string op = _currentToken.token;
            NextToken();

            AstNode right = null;

            if (_currentToken.code == 1)
            {
                string name = _currentToken.token;

                if (_symbolTable.Lookup(name) == null)
                {
                    AddError($"Переменная {name} не объявлена",
                        _currentToken.line_number,
                        _currentToken.start_pos,
                        _currentToken.end_pos);
                }

                right = new VariableNode { Name = name };
            }
            else if (_currentToken.code == 4)
            {
                int value = int.Parse(_currentToken.token);

                if (value < int.MinValue || value > int.MaxValue)
                {
                    AddError("Число вне диапазона",
                        _currentToken.line_number,
                        _currentToken.start_pos,
                        _currentToken.end_pos);
                }

                right = new LiteralNode { Value = value };
            }

            NextToken();

            if (_currentToken.code == 12)
            {
                NextToken();
                _errorCounter = 0;
            }

            return new BinaryOpNode
            {
                Left = left,
                Operator = op,
                Right = right
            };
        }

        private BlockNode Block()
        {
            var block = new BlockNode();

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается открывающая фигурная скобка",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return block;
            }

            if (_currentToken.code == 9)
            {
                NextToken();
                _errorCounter = 0;
            }

            var stmt = Statement();
            if (stmt != null)
                block.Statements.Add(stmt);

            if (_currentToken.code == 10)
            {
                NextToken();
                _errorCounter = 0;
            }

            return block;
        }

        private AstNode Statement()
        {
            if (!IsValidToken())
            {
                endFlag = true;
                return null;
            }

            string varName = null;

            if (_currentToken.code == 1)
            {
                varName = _currentToken.token;

                if (_symbolTable.Lookup(varName) == null)
                {
                    AddError($"Переменная {varName} не объявлена",
                        _currentToken.line_number,
                        _currentToken.start_pos,
                        _currentToken.end_pos);
                }

                NextToken();
                _errorCounter = 0;
            }

            string op = null;

            if (increment_operation.Contains(_currentToken.code))
            {
                op = _currentToken.token;
                NextToken();
                _errorCounter = 0;
            }

            if (_currentToken.code == 17)
            {
                NextToken();
                _errorCounter = 0;
            }

            return new UnaryOpNode
            {
                Operator = op,
                Variable = new VariableNode { Name = varName }
            };
        }

        //private DoWhileNode DoWhileStatement()
        //{
        //    if (_currentToken.code == 3) // do
        //        NextToken();

        //    var block = Block();

        //    if (_currentToken.code == 2) // while
        //        NextToken();

        //    var condition = Condition();

        //    if (_currentToken.code == 17) // ;
        //        NextToken();

        //    return new DoWhileNode
        //    {
        //        Body = block,
        //        Condition = condition
        //    };
        //}

        //private AstNode Condition()
        //{
        //    if (_currentToken.code == 11) // (
        //        NextToken();

        //    AstNode left = null;

        //    if (_currentToken.code == 1)
        //    {
        //        string name = _currentToken.token;
        //        if (_symbolTable.Lookup(name) == null)
        //        {
        //            AddError($"Переменная {name} не объявлена",
        //                _currentToken.line_number,
        //                _currentToken.start_pos,
        //                _currentToken.end_pos);
        //        }

        //        left = new VariableNode { Name = name };
        //    }
        //    else if (_currentToken.code == 4)
        //    {
        //        left = new LiteralNode { Value = int.Parse(_currentToken.token) };
        //    }

        //    NextToken();

        //    string op = _currentToken.token;
        //    NextToken();

        //    AstNode right = null;

        //    if (_currentToken.code == 1)
        //    {
        //        string name = _currentToken.token;

        //        if (_symbolTable.Lookup(name) == null)
        //        {
        //            AddError($"Переменная {name} не объявлена",
        //                _currentToken.line_number,
        //                _currentToken.start_pos,
        //                _currentToken.end_pos);
        //        }

        //        right = new VariableNode { Name = name };
        //    }
        //    else if (_currentToken.code == 4)
        //    {
        //        int value = int.Parse(_currentToken.token);

        //        if (value < int.MinValue || value > int.MaxValue)
        //        {
        //            AddError("Число вне диапазона",
        //                _currentToken.line_number,
        //                _currentToken.start_pos,
        //                _currentToken.end_pos);
        //        }

        //        right = new LiteralNode { Value = value };
        //    }

        //    NextToken();

        //    if (_currentToken.code == 12) // )
        //        NextToken();

        //    return new BinaryOpNode
        //    {
        //        Left = left,
        //        Operator = op,
        //        Right = right
        //    };
        //}

        //private BlockNode Block()
        //{
        //    var block = new BlockNode();

        //    if (_currentToken.code == 9) // {
        //        NextToken();

        //    var stmt = Statement();
        //    if (stmt != null)
        //        block.Statements.Add(stmt);

        //    if (_currentToken.code == 10) // }
        //        NextToken();

        //    return block;
        //}

        //private AstNode Statement()
        //{
        //    if (!IsValidToken())
        //    {
        //        endFlag = true;
        //        return null;
        //    }

        //    string varName = null;

        //    if (_currentToken.code == 1)
        //    {
        //        varName = _currentToken.token;
        //        if (_symbolTable.Lookup(varName) == null)
        //        {
        //            AddError($"Переменная {varName} не объявлена",
        //                _currentToken.line_number,
        //                _currentToken.start_pos,
        //                _currentToken.end_pos);
        //        }

        //        NextToken();
        //    }

        //    string op = null;

        //    if (increment_operation.Contains(_currentToken.code))
        //    {
        //        op = _currentToken.token;
        //        NextToken();
        //    }

        //    if (_currentToken.code == 17)
        //    {
        //        NextToken();
        //    }
        //    return new UnaryOpNode
        //    {
        //        Operator = op,
        //        Variable = new VariableNode { Name = varName }
        //    };
        //}
    }


    public class ParseError
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public override string ToString()
        {
            return $"Строка {Line + 1}, позиция {StartPosition}-{EndPosition}: {Message}";
        }
    }

}
