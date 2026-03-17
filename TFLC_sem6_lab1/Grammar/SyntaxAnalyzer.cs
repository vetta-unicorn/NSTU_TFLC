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

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _currentToken = _tokens.Count > 0 ? _tokens[0] : null;
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
        private void Program()
        {
            DoWhileStatement();
        }

        private void DoWhileStatement()
        {
            if (_currentToken.code == 2)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается do",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            Block();

            if (_currentToken.code == 3)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается while",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            Condition();

            if (_currentToken.code == 17)
            {
                NextToken();
            }
            else
            {
                AddError($"Ожидается ;",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }
        }

        private void Condition()
        {
            if (_currentToken.code == 11)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается (",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 1)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается id",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 13 || _currentToken.code == 14 ||
                _currentToken.code == 15 || _currentToken.code == 16 ||
                _currentToken.code == 19 || _currentToken.code == 20)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается relation operation",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 4)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается число",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 12)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается )",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }
        }

        private void Block()
        {
            if (_currentToken.code == 9)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается {", 
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            Statement();

            if (_currentToken.code == 10)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается }",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

        }

        private void Statement()
        {
            if (_currentToken.code == 1)
            {
                NextToken();
            }
            else
            {
                AddError("Ожидается id",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 6 || _currentToken.code == 8)
            {
                NextToken();
            }
            else
            {
                AddError($"Ожидается ++ или --",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

            if (_currentToken.code == 17)
            {
                NextToken();
            }
            else
            {
                AddError($"Ожидается ;",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                return;
            }

        }

    }

    public class ParseError
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition {  get; set; }

        public override string ToString()
        {
            return $"Строка {Line + 1}, позиция {StartPosition}-{EndPosition}: {Message}";
        }
    }

}
