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
        private int[] relation_operation = new int[] { 13, 14, 15, 16, 19, 20 };
        private int[] increment_operation = new int[] { 6, 8 };
        private int _errorCounter;

        private bool endFlag = false;

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
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

        private bool SkipToSynchronizingToken(int[] codes)
        {
            int pos = _currentPos;
            int i = 0;
            while (IsValidToken() && i < 4)
            {
                if (codes.Contains(_currentToken.code))
                {
                    return true;
                }
                NextToken();
                i++;
            }
            _currentPos = pos;
            _currentToken = _tokens[pos];
            return false;
        }

        private bool SkipToToken(int[] neededCodes)
        {
            int i = 0;
            while (IsValidToken() && i < 3)
            {
                if (neededCodes.Contains(_currentToken.code))
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        private void ExpectToken(int[] codes, int[] syncro, string message)
        {
            ExpectEOF(message);

            if (endFlag) return;

            if (codes.Contains(_currentToken.code))
            {
                NextToken();
                _errorCounter = 0;
            }
            else
            {
                if (_errorCounter > 5) { endFlag = true; return; }
                AddError(message,
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                SkipToSynchronizingToken(syncro);
                if (SkipToToken(codes))
                {
                    _errors.Last().Message = "Неожиданный токен";
                    NextToken();
                }
                _errorCounter++;
            }
        }

        private void ExpectEOF(string message)
        {
            if (!IsValidToken())
            {
                AddError(message,
                    _tokens[Math.Max(0, _currentPos - 1)].line_number,
                    _tokens[Math.Max(0, _currentPos - 1)].start_pos,
                    _tokens[Math.Max(0, _currentPos - 1)].end_pos);
                endFlag = true;
                return;
            }
        }

        private void Program()
        {
            if (!IsValidToken()) return;

            DoWhileStatement();

            if (endFlag) return;
            if (IsValidToken())
            {
                AddError($"Ошибочный токен в конце строки",
                    _currentToken.line_number, 
                    _currentToken.start_pos, _currentToken.end_pos);
            }
        }
        private void DoWhileStatement()
        {
            if (!IsValidToken()) return;

            ExpectToken([3], [3, 9, 1, 6, 8, 17],
                "В конструкции do-while: ожидается ключевое слово do");

            if (endFlag) return;
            Block();

            if (endFlag) return;
            ExpectToken([2], [2, 11, 1, 13, 14, 15, 16, 19, 20, 4],
                "В конструкции do-while: ожидается ключевое слово while");

            if (endFlag) return;
            Condition();

            if (endFlag) return;
            ExpectToken([17], [17], "Ожидается завершающая точка с запятой");
        }

        private void Condition()
        {
            if (endFlag) return;
            ExpectToken([11], [11, 1, 13, 14, 15, 16, 19, 20, 4, 12],
                "В условном выражении: ожидается открывающая круглая скобка");

            if (endFlag) return;
            ExpectToken([1, 4], [1, 13, 14, 15, 16, 19, 20, 4, 12, 17],
                "В условном выражении: ожидается идентификатор или число");

            if (endFlag) return;
            ExpectToken(relation_operation, [13, 14, 15, 16, 19, 20, 4, 12, 17],
                "В условном выражении: ожидается оператор сравнения");

            if (endFlag) return;
            ExpectToken([1, 4], [1, 4, 12, 17],
                "В условном выражении: ожидается идентификатор или число");

            if (endFlag) return;
            ExpectToken([12], [12, 17],
                "В условном выражении: ожидается закрывающая круглая скобка");

            if (endFlag) return;
            ExpectEOF("Ожидается завершающая точка с запятой");
        }

        private void Block()
        {
            if (endFlag) return;
            ExpectToken([9], [9, 1, 6, 8, 17, 10],
                "В блоке: ожидается открывающая фигурная скоба");

            if (endFlag) return;
            Statement();

            if (endFlag) return;
            ExpectToken([10], [10, 2, 11, 1, 13, 14, 15, 16, 19, 20],
                "В блоке: ожидается закрывающая фигурная скоба");

            if (endFlag) return;
            ExpectEOF("В конструкции do-while: ожидается ключевое слово while");
        }

        private void Statement()
        {
            if (endFlag) return;
            ExpectToken([1], [1, 6, 8, 17, 10, 2],
                "В блоке: ожидается идентификатор");

            if (endFlag) return;
            ExpectToken(increment_operation, [6, 8, 17, 10, 2, 11],
                "В блоке: ожидается оператор инкремента или декремента");

            if (endFlag) return;
            ExpectToken([17], [17, 10, 2, 11, 1, 4],
                "В блоке: ожидается точка с запятой");

            if (endFlag) return;
            ExpectEOF("В блоке: ожидается закрывающая фигурная скоба");
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
