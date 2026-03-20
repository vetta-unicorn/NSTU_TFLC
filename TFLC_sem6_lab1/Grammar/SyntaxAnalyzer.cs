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

        public Parser(List<TableLine> tokens)
        {
            _tokens = tokens;
            _currentPos = 0;
            _errors = new List<ParseError>();
            _tokenDict = new TokenDict();
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

        private void ExpectToken(int code)
        {
            if (_currentToken.code == code)
            {
                NextToken();
            }
            else
            {
                AddError($"Ожидается {_tokenDict.tokens[code]}",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                Neutralize(code);
            }
        }

        private void ExpectToken(int[] codes)
        {
            bool found = false;
            foreach (int code in codes)
            {
                if (_currentToken.code == code)
                {
                    NextToken();
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                AddError($"Ожидается {_tokenDict.tokens[codes[0]]}",
                    _currentToken.line_number,
                    _currentToken.start_pos, _currentToken.end_pos);
                Neutralize(codes[0]);
            }
        }

        private void Neutralize(int code)
        {
            bool _found = false;
            for (int i = _currentPos; i < _tokens.Count && i < _currentPos + 5; i++)
            {
                if (_tokens[i].code == code)
                {
                    _currentPos = i;
                    _currentToken = _tokens[_currentPos];
                    NextToken();
                    _found = true;
                    return;
                }
            }
            if (!_found)
            {
                _tokens.Insert(_currentPos, new TableLine(code,
                    _currentToken.line_number, _currentToken.start_pos,
                    _currentToken.end_pos));
                NextToken();
            }
        }
        private void Program()
        {
            DoWhileStatement();
        }

        private void DoWhileStatement()
        {
            if (_currentPos >= _tokens.Count) return;
            if (_currentToken == null) return;

            ExpectToken(3); // do

            Block();
            if (_currentPos > _tokens.Count) return;

            ExpectToken(2); // while

            Condition();
            if (_currentPos > _tokens.Count) return;

            ExpectToken(17); // ;
        }


        private void Condition()
        {
            if (_currentPos >= _tokens.Count) return;
            if (_currentToken == null) return;

            ExpectToken(11); // (

            ExpectToken(1); // id

            ExpectToken(relation_operation); // relation operation 

            ExpectToken(4); //digit

            ExpectToken(12); // )
        }

        private void Block()
        {
            if (_currentPos >= _tokens.Count) return;
            if (_currentToken == null) return;

            ExpectToken(9); // {

            Statement();
            if (_currentPos > _tokens.Count) return;

            ExpectToken(10); // }

        }


        private void Statement()
        {
            if (_currentPos >= _tokens.Count) return;
            if (_currentToken == null) return;

            ExpectToken(1); // id

            ExpectToken(6); // -- ++

            ExpectToken(17); // ;

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
