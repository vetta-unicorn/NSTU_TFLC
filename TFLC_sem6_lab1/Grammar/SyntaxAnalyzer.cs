using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TFLC_sem6_lab1.Scanner;

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

        private bool endFlag = false;

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _tokenDict = new TokenDict();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
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
                Program();
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

        private bool MatchToken(int code)
        {
            if (IsValidToken() && _currentToken.code == code)
            {
                NextToken();
                return true;
            }
            return false;
        }

        private void ExpectToken(int code, string context)
        {
            if (MatchToken(code))
                return;

            AddError($"{context}: ожидается {_tokenDict.tokens[code]}",
                _currentToken?.line_number ?? 0,
                _currentToken?.start_pos ?? 0,
                _currentToken?.end_pos ?? 0);
        }

        private bool SkipToToken(int code)
        {
            int pos = _currentPos;
            while (IsValidToken() && _currentToken.code != code)
            {
                NextToken();
                if (_currentToken != null)
                {
                    if (_currentToken.code == code)
                    {
                        NextToken();
                        return true;
                    }
                }
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private bool SkipToToken(int[] codes)
        {
            int pos = _currentPos;
            while (IsValidToken() && !codes.Contains(_currentToken.code))
            {
                NextToken();
                if (_currentToken != null)
                {
                    if (codes.Contains(_currentToken.code)) return true;
                }
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private void SkipToSynchronizingToken()
        {
            while (IsValidToken())
            {
                if (_currentToken.code == 3 ||  // do
                    _currentToken.code == 2 ||  // while
                    _currentToken.code == 9 ||  // {
                    _currentToken.code == 10 || // }
                    _currentToken.code == 17)   // ;
                {
                    return;
                }
                NextToken();
            }
        }

        private void SkipToSynchronizingToken(int[] codes)
        {
            int pos = _currentPos;
            while (IsValidToken())
            {
                if (codes.Contains(_currentToken.code))
                {
                    return;
                }
                NextToken();
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
        }

        private void Program()
        {
            if (!IsValidToken()) return;

            DoWhileStatement();

            if (IsValidToken())
            {
                AddError($"Неизвестный токен в конце строки: " +
                    $"{_currentToken.token}",_currentToken.line_number, 
                    _currentToken.start_pos, _currentToken.end_pos);
            }
        }

        private void DoWhileStatement()
        {
            if (!IsValidToken()) return;

            if (_currentToken.code != 3)
            {
                AddError("Ошибочный токен в начале строки",
                    _tokens[_currentPos].line_number,
                    _tokens[_currentPos].start_pos,
                    _tokens[_currentPos].end_pos);
                if (SkipToToken(3))
                {
                    SkipToToken(3);
                }
                else
                {
                    SkipToSynchronizingToken([3, 9, 1]); // do { $id
                }
            }

            if (!IsValidToken())
            {
                AddError("В конструкции do-while ожидается do",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 3)
            {
                NextToken();
            }
            else
            {
                AddError($"В конструкции do-while: ожидается ключевое слово do",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(3)) SkipToToken(3);
                else SkipToSynchronizingToken([3, 9, 1]); // do { $id
            }

            if (!IsValidToken() || _tokens.Count == 1)
            {
                AddError("В условном выражении: ожидается открывающая фигурная скобка",
                    _tokens[0].line_number,
                    _tokens[0].start_pos,
                    _tokens[0].end_pos);
                NextToken();
                return;
            }

            Block();
            if (endFlag) return;

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается ключевое слово while",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                return;
            }

            if (_currentToken.code == 2)
            {
                NextToken();
            }
            else
            {
                AddError($"В конструкции do-while: ожидается ключевое слово while",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(2))
                {
                    SkipToToken(2);
                }
                else
                {
                    SkipToSynchronizingToken([2, 11, 1]); // while ( $id
                }
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается (",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            Condition();

            if (endFlag) return;

            if (IsValidToken() && _currentToken.code == 17)
                NextToken();
            else
            {
                AddError($"Ожидается завершающая точка с запятой",
                   _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
            }
        }

        private void Condition()
        {
            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается открывающая круглая скобка",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 11)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается открывающая круглая скобка",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(11)) SkipToToken(11);
                else SkipToSynchronizingToken([11, 1, 13, 14, 15, 16, 19, 20]); // ( $id >
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается идентификатор или число",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 1 || _currentToken.code == 4)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается идентификатор или число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken([1, 4])) SkipToToken([1, 4]);
                else SkipToSynchronizingToken([1, 13, 14, 15, 16, 19, 20, 4]); // $id > num
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается оператор сравнения",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (relation_operation.Contains(_currentToken.code))
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается оператор сравнения",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(relation_operation)) 
                    SkipToToken(relation_operation);
                else SkipToSynchronizingToken([13, 14, 15, 16, 19, 20, 4, 12]); // > num )
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается идентификатор или число",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 1 || _currentToken.code == 4)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается идентификатор или число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken([1, 4])) SkipToToken([1, 4]);
                else SkipToSynchronizingToken([1, 4, 12, 17]); // $id | num ) ;
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается )",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 12)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается закрывающая круглая скобка",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(12)) SkipToToken(12);
                else SkipToSynchronizingToken([12, 17]); // ) ;
            }
        }

        private void Block()
        {
            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается открывающая фигурная скобка",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 9)
            {
                NextToken();
            }
            else
            {
                AddError($"В блоке: ожидается открывающая фигурная скоба",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(9)) SkipToToken(9);
                else SkipToSynchronizingToken([9, 1, 6, 8]); // { $id ++ | --
            }


            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается идентификатор",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            Statement();

            if (endFlag) return;

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается закрывающая фигурная скобка",
                        _tokens[_currentPos - 1].line_number,
                        _tokens[_currentPos - 1].start_pos,
                        _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 10)
            {
                NextToken();
            }

            else
            {
                AddError($"В блоке: ожидается закрывающая фигурная скоба",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(10)) SkipToToken(10);
                else SkipToSynchronizingToken([10, 2, 11]); // } while (
            }

            if (!IsValidToken())
            {
                endFlag = true;
            }
        }

        private void Statement()
        {
            if (!IsValidToken())
            {
                endFlag = true;
                return;
            }

            if (!IsValidToken())
            {
                AddError("В условном выражении: ожидается идентификатор",
                    _tokens[_currentPos - 1].line_number,
                    _tokens[_currentPos - 1].start_pos,
                    _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 1)
            {
                NextToken();
            }

            else
            {
                AddError($"В блоке: ожидается идентификатор",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(1)) SkipToToken(1);
                else SkipToSynchronizingToken([1, 6, 8, 17]); // $id ++ | -- ;
            }

            if (!IsValidToken())
            {
                AddError("В блоке: ожидается оператор инкремента или декремента",
                        _tokens[_currentPos - 1].line_number,
                        _tokens[_currentPos - 1].start_pos,
                        _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (increment_operation.Contains(_currentToken.code))
            {
                NextToken();
            }

            else
            {
                AddError($"В блоке: ожидается оператор инкремента или декремента",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(increment_operation)) 
                    SkipToToken(increment_operation);
                else SkipToSynchronizingToken([6, 8, 17, 10]); // ++ | -- ; }
            }

            if (!IsValidToken())
            {
                AddError("В блоке: ожидается точка с запятой",
                        _tokens[_currentPos - 1].line_number,
                        _tokens[_currentPos - 1].start_pos,
                        _tokens[_currentPos - 1].end_pos);
                endFlag = true;
                return;
            }

            if (_currentToken.code == 17)
            {
                NextToken();
            }

            else
            {
                AddError($"В блоке: ожидается точка с запятой",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                if (SkipToToken(17)) SkipToToken(17);
                else SkipToSynchronizingToken([17, 10, 2]); // ; } while
            }

            if (!IsValidToken())
            {
                endFlag = true;
            }
        }
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
