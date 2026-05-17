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
        private int[] relation_operation = new int[] { 13, 14, 15, 16, 19, 20 };
        private int[] increment_operation = new int[] { 6, 8 };
        private int _errorCounter;
        private bool endFlag = false;
        public AstNode Root { get; private set; }
        private SymbolTable _symbolTable = new SymbolTable();

        private List<TacInstruction> _tacInstructions; 
        private int _tempCounter; 
        private int _labelCounter;

        TableLine statementToken;
        TableLine condition1Token;
        TableLine condition2Token;

        ParseError _semanticError;

        private string _optimizationStats;
        public string GetOptimizationStats() => _optimizationStats;

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;

            _tacInstructions = new List<TacInstruction>();
            _tempCounter = 0;
            _labelCounter = 0;
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

        public (List<ParseError>, AstNode, ParseError, List<TacInstruction>) Parse()
        {
            try
            {
                Root = Program();
            }
            catch (Exception ex)
            {
                AddError($"Критическая ошибка: {ex.Message}", 0, 0, 0);
            }

            return (_errors, Root, _semanticError, _tacInstructions);
        }

        public List<TacInstruction> GetOptimizedTAC()
        {
            if (_tacInstructions == null || _tacInstructions.Count == 0)
                return new List<TacInstruction>();

            var optimizer = new TACOptimizer();
            var optimized = optimizer.Optimize(new List<TacInstruction>(_tacInstructions));

            _optimizationStats = optimizer.GetOptimizationStats();

            return optimized;
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

            if (statementToken.token != condition1Token.token 
                && statementToken.token != condition2Token.token)
            {
                _semanticError = new ParseError("В условии должен быть хотя бы один " +
                    "идентификатор из блока", condition1Token.line_number,
                    condition1Token.start_pos, condition1Token.end_pos);
            }

            return node;
        }

        private string GenerateConditionTAC(AstNode condition)
        {
            if (condition is BinaryOpNode binaryOp)
            {
                string leftVal = GenerateExpressionTAC(binaryOp.Left);

                string rightVal = GenerateExpressionTAC(binaryOp.Right);

                string result = NewTemp();

                AddTacInstruction(binaryOp.Operator, leftVal, rightVal, result);

                return result;
            }

            return "";
        }

        private string GenerateExpressionTAC(AstNode expr)
        {
            if (expr is VariableNode varNode)
            {
                return varNode.Name;
            }
            else if (expr is LiteralNode litNode)
            {
                return litNode.Value.ToString();
            }

            return "";
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

            string startLabel = NewLabel();  
            string condLabel = NewLabel();   
            string endLabel = NewLabel();    

            AddTacInstruction("label", result: startLabel);

            blockNode = Block();
            if (endFlag) return null;

            AddTacInstruction("label", result: condLabel);

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

            conditionNode = Condition();
            if (endFlag) return null;

            string condResult = GenerateConditionTAC(conditionNode);

            AddTacInstruction("jz", condResult, "", endLabel);

            AddTacInstruction("jmp", result: startLabel);

            AddTacInstruction("label", result: endLabel);

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
                    AddError($"В условном выражении: ожидается открывающая круглая скобка",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(11)) SkipToToken(11);
                else SkipToSynchronizingToken([11, 1, 13, 14, 15, 16, 19, 20]); // ( $id >
                _errorCounter++;
            }

            AstNode left = null;

            if (_currentToken.code == 1)
            {
                string name = _currentToken.token;

                left = new VariableNode { Name = name };
                condition1Token = _currentToken;
                NextToken();
                _errorCounter = 0;
            }
            else if (_currentToken.code == 4)
            {
                left = new LiteralNode { Value = int.Parse(_currentToken.token) };
                condition1Token = _currentToken;
                NextToken();
                _errorCounter = 0;
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается операция сравнения",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            string op = _currentToken.token;

            AstNode right = null;

            if (relation_operation.Contains(_currentToken.code))
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
                    AddError($"В условном выражении: ожидается оператор сравнения",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(relation_operation))
                    SkipToToken(relation_operation);
                else SkipToSynchronizingToken([13, 14, 15, 16, 19, 20, 4, 12]); // > num )
                _errorCounter++;
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается идентификатор или число",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            if (_currentToken.code == 1)
            {
                string name = _currentToken.token;
                right = new VariableNode { Name = name };
                condition2Token = _currentToken;
                NextToken();
                _errorCounter = 0;
            }
            else if (_currentToken.code == 4)
            {
                int value = int.Parse(_currentToken.token);
                condition2Token = _currentToken;
                right = new LiteralNode { Value = value };
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
                    AddError($"В условном выражении: ожидается идентификатор или число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken([1, 4])) SkipToToken([1, 4]);
                else SkipToSynchronizingToken([1, 4, 12, 17]); // $id | num ) ;
                _errorCounter++;
            }

            if (_currentToken.code == 12)
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
                    AddError($"В условном выражении: ожидается закрывающая круглая скобка",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(12)) SkipToToken(12);
                else SkipToSynchronizingToken([12, 17]); // ) ;
                _errorCounter++;
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
                    AddError($"В блоке: ожидается открывающая фигурная скоба",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(9)) SkipToToken(9);
                else SkipToSynchronizingToken([9, 1, 6, 8]); // { $id ++ | --
                _errorCounter++;
            }

            var stmt = Statement();
            if (stmt != null)
                block.Statements.Add(stmt);

            if (!IsValidToken())
            {
                AddError("В блоке: ожидается закрывающая фигурная скобка",
                        _tokens[_currentPos - 1].line_number,
                        _tokens[_currentPos - 1].start_pos,
                        _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            if (_currentToken.code == 10)
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
                    AddError($"В блоке: ожидается закрывающая фигурная скоба",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(10)) SkipToToken(10);
                else SkipToSynchronizingToken([10, 2, 11]); // } while (
                _errorCounter++;
            }


            if (!IsValidToken())
            {
                endFlag = true;
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
                statementToken = _currentToken;
                varName = _currentToken.token;
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
                    AddError($"В блоке: ожидается идентификатор",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(1)) SkipToToken(1);
                else SkipToSynchronizingToken([1, 6, 8, 17]); // $id ++ | -- ;
                _errorCounter++;
            }

            string op = null;

            if (increment_operation.Contains(_currentToken.code))
            {
                op = _currentToken.token;
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
                    AddError($"В блоке: ожидается оператор инкремента или декремента",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                }
                if (SkipToToken(increment_operation))
                    SkipToToken(increment_operation);
                else SkipToSynchronizingToken([6, 8, 17, 10]); // ++ | -- ; }
                _errorCounter++;
            }

            if (!IsValidToken())
            {
                AddError("В блоке: ожидается точка с запятой",
                        _tokens[_currentPos - 1].line_number,
                        _tokens[_currentPos - 1].start_pos,
                        _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return null;
            }

            if (_currentToken.code == 17)
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
                    AddError($"В блоке: ожидается точка с запятой",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                }
                if (SkipToToken(17)) SkipToToken(17);
                else SkipToSynchronizingToken([17, 10, 2]); // ; } while
                _errorCounter++;
            }

            if (!IsValidToken())
            {
                endFlag = true;
            }

            if (varName != null && op != null)
            {
                if (op == "++")
                {
                    string temp = NewTemp();
                    AddTacInstruction("+", varName, "1", temp);
                    AddTacInstruction("=", temp, "", varName);
                }
                else if (op == "--")
                {
                    string temp = NewTemp();
                    AddTacInstruction("-", varName, "1", temp);
                    AddTacInstruction("=", temp, "", varName);
                }
            }

            return new UnaryOpNode
            {
                Operator = op,
                Variable = new VariableNode { Name = varName }
            };
        }

        private string NewTemp()
        {
            return $"t{_tempCounter++}";
        }

        private string NewLabel()
        {
            return $"L{_labelCounter++}";
        }

        private void AddTacInstruction(string op, string arg1 = "", string arg2 = "", string result = "")
        {
            _tacInstructions.Add(new TacInstruction(op, arg1, arg2, result));
        }

        public string GetTACString()
        {
            if (_tacInstructions == null || _tacInstructions.Count == 0)
                return "TAC не сгенерирован";

            return string.Join(Environment.NewLine, _tacInstructions.Select(i => i.ToString()));
        }

    }


    public class ParseError
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public ParseError() { }

        public ParseError(string message, int line, int startPosition, int endPosition)
        {
            Message = message;
            Line = line;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public override string ToString()
        {
            return $"Строка {Line + 1}, позиция {StartPosition}-{EndPosition}: {Message}";
        }
    }
}
