using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
    public class SMSurveyReportRow
    {
        private string _stateName = string.Empty;
        private long _questionID = -1;
        private long _languageID = -1;
        private long _matrixIndex = -1;
        private long _displayIndex = -1;
        private string _questionContent = string.Empty;
        private string _answerContent = string.Empty;
        private long _responseCount = -1;
        private long _groupTotal = 0;

        public SMSurveyReportRow()
        {
        }

        public SMSurveyReportRow(string stateName, long questionID, long languageID, long matrixIndex,
            long displayIndex, string questionContent, string answerContent, long responseCount)
        {
            _stateName = stateName;
            _questionID = questionID;
            _languageID = languageID;
            _matrixIndex = matrixIndex;
            _displayIndex = displayIndex;
            _questionContent = questionContent;
            _answerContent = answerContent;
            _responseCount = responseCount;
        }

        public string GetKey()
        {
            string key = _stateName + "_" + _questionID.ToString() + "_" + _languageID.ToString() + "_" + _matrixIndex.ToString();
            return key;
        }

        public string StateName {
            get { return _stateName; }
            set { _stateName = value; }
        }

        public long QuestionID
        {
            get { return _questionID; }
            set { _questionID = value; }
        }

        public long LanguageID
        {
            get { return _languageID; }
            set { _languageID = value; }
        }

        public long MatrixIndex
        {
            get { return _matrixIndex; }
            set { _matrixIndex = value; }
        }

        public long DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = value; }
        }

        public string QuestionContent
        {
            get { return _questionContent; }
            set { _questionContent = value; }
        }

        public string AnswerContent
        {
            get { return _answerContent; }
            set { _answerContent = value; }
        }

        public long ResponseCount
        {
            get { return _responseCount; }
            set { _responseCount = value; }
        }

        public long GroupTotal
        {
            get { return _groupTotal; }
            set { _groupTotal = value; }
        }
    }
}
