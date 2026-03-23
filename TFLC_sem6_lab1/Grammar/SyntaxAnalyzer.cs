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
        private int[] expression_start = new int[] { 1, 4 }; 
        private bool _errorReported;

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _tokenDict = new TokenDict();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
            _errorReported = false;
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

        private bool IsErrorToken()
        {
            return IsValidToken() && _currentToken.code == -1;
        }

        private bool MatchToken(int code)
        {
            if (IsValidToken() && !IsErrorToken() && _currentToken.code == code)
            {
                NextToken();
                return true;
            }
            return false;
        }

        private bool MatchToken(int[] codes)
        {
            if (IsValidToken() && !IsErrorToken())
            {
                foreach (int code in codes)
                {
                    if (_currentToken.code == code)
                    {
                        NextToken();
                        return true;
                    }
                }
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

        private void ExpectToken(int[] codes, string context)
        {
            if (MatchToken(codes))
                return;

            string expected = string.Join(" или ", codes.Select(c => _tokenDict.tokens[c]));
            AddError($"{context}: ожидается {expected}",
                _currentToken?.line_number ?? 0,
                _currentToken?.start_pos ?? 0,
                _currentToken?.end_pos ?? 0);
        }

        private void SkipToToken(int code)
        {
            while (IsValidToken() && _currentToken.code != code)
            {
                NextToken();
            }
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

        private void Program()
        {
            if (!IsValidToken()) return;

            DoWhileStatement();

            while (IsValidToken())
            {
                if (IsErrorToken())
                {
                    AddError($"Неизвестный токен: {_currentToken.token}",
                        _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                    NextToken();
                }
                else
                {
                    break;
                }
            }
        }

        private void DoWhileStatement()
        {
            if (!IsValidToken()) return;

            ExpectToken(3, "В конструкции do-while");

            if (!IsValidToken()) return;

            Block();

            if (!IsValidToken()) return;

            if (_currentToken.code == 2)
            {
                NextToken();
            }
            else
            {
                AddError($"В конструкции do-while: ожидается ключевое слово while",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);

                SkipToToken(2);
                if (IsValidToken() && _currentToken.code == 2)
                {
                    NextToken();
                }
            }

            if (!IsValidToken()) return;

            Condition();

            if (IsValidToken() && _currentToken.code == 17)
                NextToken();
        }

        private void Condition()
        {
            if (!IsValidToken()) return;

            if (_currentToken.code == 11)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается (",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken();
                return;
            }

            if (!IsValidToken()) return;

            if (_currentToken.code == 1 || _currentToken.code == 4)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается идентификатор или число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken();
                return;
            }

            if (!IsValidToken()) return;

            if (relation_operation.Contains(_currentToken.code))
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается оператор сравнения",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken();
                return;
            }

            if (!IsValidToken()) return;

            if (_currentToken.code == 1 || _currentToken.code == 4)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается идентификатор или число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken();
                return;
            }

            if (!IsValidToken()) return;

            if (_currentToken.code == 12)
            {
                NextToken();
            }
            else
            {
                AddError($"В условном выражении: ожидается )",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken();
            }
        }

        private void Block()
        {
            if (!IsValidToken()) return;

            _errorReported = false;

            if (_currentToken.code == 9)
            {
                NextToken();
            }
            else
            {
                AddError($"В блоке: ожидается открывающая фигурная скоба",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                _errorReported = true;
            }

            while (IsValidToken() && _currentToken.code != 10 && _currentToken.code != 2)
            {
                if (IsErrorToken())
                {
                    AddError($"Неизвестный токен: {_currentToken.token}",
                        _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);
                    NextToken();
                    continue;
                }
                Statement();
            }

            if (IsValidToken())
            {
                if (_currentToken.code == 10)
                {
                    NextToken();
                }
                else if (_currentToken.code == 2)
                {
                    if (!_errorReported)
                    {
                        AddError($"В блоке: ожидается закрывающая фигурная скоба",
                            _currentToken.line_number - 1,
                            _currentToken.start_pos, _currentToken.end_pos);
                    }
                }
            }
            else if (!_errorReported)
            {
                AddError($"В блоке: ожидается закрывающая фигурная скоба",
                    _currentToken?.line_number ?? 0,
                    _currentToken?.start_pos ?? 0,
                    _currentToken?.end_pos ?? 0);
            }
        }

        private void Statement()
        {
            if (!IsValidToken()) return;

            if (IsErrorToken())
            {
                AddError($"Неизвестный токен: {_currentToken.token}",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                NextToken();
                return;
            }

            if (_currentToken.code == 10 || _currentToken.code == 2)
                return;
            if (_currentToken.code == 1)
            {
                NextToken();
            }
            else
            {
                AddError($"В операторе: ожидается идентификатор",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);

                while (IsValidToken() && _currentToken.code != 17 && _currentToken.code != 10 && _currentToken.code != 2)
                {
                    NextToken();
                }

                if (IsValidToken() && _currentToken.code == 17)
                {
                    NextToken();
                }
                return;
            }

            if (IsValidToken() && (_currentToken.code == 6 || _currentToken.code == 8)) // ++ or --
            {
                NextToken();
            }
            else if (IsValidToken() && _currentToken.code != 17 && _currentToken.code != 10 && _currentToken.code != 2)
            {
                AddError($"В операторе: ожидается ++ или --",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);

                while (IsValidToken() && _currentToken.code != 17 && _currentToken.code != 10 && _currentToken.code != 2)
                {
                    NextToken();
                }
            }

            if (IsValidToken())
            {
                if (_currentToken.code == 17)
                {
                    NextToken();
                }
                else if (_currentToken.code != 10 && _currentToken.code != 2)
                {
                    AddError($"В операторе: ожидается ;",
                        _currentToken.line_number,
                        _currentToken.start_pos, _currentToken.end_pos);

                    while (IsValidToken() && _currentToken.code != 17 && _currentToken.code != 10 && _currentToken.code != 2)
                    {
                        NextToken();
                    }

                    if (IsValidToken() && _currentToken.code == 17)
                    {
                        NextToken();
                    }
                }
            }
        }
    }

    //public class Parser
    //{
    //    private List<TableLine> _tokens;
    //    private int _currentPos;
    //    private TableLine _currentToken;
    //    private List<ParseError> _errors;
    //    private TokenDict _tokenDict;
    //    private int[] relation_operation = new int[] { 13, 14, 15, 16, 19, 20 };

    //    private int _errorPos;

    //    public Parser(List<TableLine> tokens)
    //    {
    //        _tokens = tokens;
    //        _currentPos = 0;
    //        _errorPos = -1;
    //        _errors = new List<ParseError>();
    //        _tokenDict = new TokenDict();
    //        _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
    //    }

    //    public List<ParseError> Parse()
    //    {
    //        try
    //        {
    //            Program();
    //        }
    //        catch (Exception ex)
    //        {
    //            AddError($"Критическая ошибка: {ex.Message}", 0, 0, 0);
    //        }

    //        return _errors;
    //    }

    //    private void NextToken()
    //    {
    //        _currentPos++;
    //        if (_currentPos < _tokens.Count)
    //            _currentToken = _tokens[_currentPos];
    //    }

    //    private void AddError(string message, int line, int start_position, int end_position)
    //    {
    //        _errors.Add(new ParseError
    //        {
    //            Message = message,
    //            Line = line,
    //            StartPosition = start_position,
    //            EndPosition = end_position
    //        });
    //    }

    //    private void ExpectToken(int code)
    //    {
    //        if (_currentToken.code == code)
    //        {
    //            NextToken();
    //        }
    //        else
    //        {
    //            AddError($"Ожидается {_tokenDict.tokens[code]}",
    //                _currentToken.line_number,
    //                _currentToken.start_pos, _currentToken.end_pos);
    //            Neutralize(code);
    //        }
    //    }

    //    private void ExpectToken(int[] codes)
    //    {
    //        bool found = false;
    //        foreach (int code in codes)
    //        {
    //            if (_currentToken.code == code)
    //            {
    //                NextToken();
    //                found = true;
    //                break;
    //            }
    //        }
    //        if (!found)
    //        {
    //            AddError($"Ожидается {_tokenDict.tokens[codes[0]]}",
    //                _currentToken.line_number,
    //                _currentToken.start_pos, _currentToken.end_pos);
    //            Neutralize(codes[0]);
    //        }
    //    }

    //    private void Neutralize(int code)
    //    {
    //        _errorPos = _currentPos;
    //        int foundPos = -1;
    //        int i = _currentPos;
    //        while (i < _tokens.Count)
    //        {
    //            if (_tokens[i].code == code)
    //            {
    //                foundPos = i;
    //            }
    //            i++;
    //        }
    //        if (foundPos == -1)
    //        {
    //            _tokens.Insert(_currentPos, new TableLine(code, _currentToken.line_number,
    //                _currentToken.start_pos, _currentToken.end_pos));
    //            NextToken();
    //            return;
    //        }
    //        else
    //        {
    //            _currentPos = foundPos;
    //            _currentToken = _tokens[_currentPos];
    //        }
    //    }

    //    private void Program()
    //    {
    //        DoWhileStatement();
    //    }

    //    private void DoWhileStatement()
    //    {
    //        if (_currentPos >= _tokens.Count) return;
    //        if (_currentToken == null) return;

    //        ExpectToken(3); // do

    //        Block();
    //        if (_currentPos > _tokens.Count) return;

    //        ExpectToken(2); // while

    //        Condition();
    //        if (_currentPos > _tokens.Count) return;

    //        ExpectToken(17); // ;
    //    }


    //    private void Condition()
    //    {
    //        if (_currentPos >= _tokens.Count) return;
    //        if (_currentToken == null) return;

    //        ExpectToken(11); // (

    //        ExpectToken(1); // id

    //        ExpectToken(relation_operation); // relation operation 

    //        ExpectToken(4); //digit

    //        ExpectToken(12); // )
    //    }

    //    private void Block()
    //    {
    //        if (_currentPos >= _tokens.Count) return;
    //        if (_currentToken == null) return;

    //        ExpectToken(9); // {

    //        Statement();
    //        if (_currentPos > _tokens.Count) return;

    //        ExpectToken(10); // }

    //    }


    //    private void Statement()
    //    {
    //        if (_currentPos >= _tokens.Count) return;
    //        if (_currentToken == null) return;

    //        ExpectToken(1); // id

    //        ExpectToken(6); // -- ++

    //        ExpectToken(17); // ;

    //    }
    //}

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
