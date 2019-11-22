using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Common.XML
{
    /// <summary>
    /// Utility methods for the survey runner for getting survey components as XML 
    /// </summary>
    public class SurveyXML
    {
        public static string MessageToXML(SMStateModel stateModel, SMMessage message)
        {
            // <message>content</message>
            XElement result = new XElement("message", message.Content);
            return result.ToString();
        }

        public static string QuestionToXML(SMStateModel stateModel, SMQuestion question)
        {
            if (question.IsMatrix)
				return MatrixQuestionToXML(stateModel, question);
            else
                return SimpleQuestionToXML(stateModel, question);
        }

        public static string SimpleQuestionToXML(SMStateModel stateModel, SMQuestion question)
        {
            return SimpleQuestionToXML(stateModel, question, true);
        }

        public static string SimpleQuestionToXML(SMStateModel stateModel, SMQuestion question, bool evaluateContent)
        {
            // <simplequestion>
            //    <body>question body content</body>
            //    <answers controltype="QA_AnswerControlType.TYPENAME" [minvalue="0" maxvalue="1"]>
            //       <answer index="0">answer body content</answer>
            //       <answer index="1">answer body content</answer>
            //    </answers>
            //    <responses>
            //       <response index="0">response content</response>
            //       <response index="1">response content</response>
            //    </responses>
            //    <otherspecified>other specified prompt content</otherspecified>
            // </simplequestion>
            //
            //  - minvalue/maxvalue only for certain control types
            //  - answer/response index is randomized as appropriate

            XElement result = new XElement("simplequestion");

            string questionBody = stateModel.GetSimpleQuestionContent(question);
            if (questionBody != null)
            {
                if (evaluateContent) questionBody = stateModel.EvaluateContent(questionBody, evaluateContent, null);
				questionBody = questionBody.Replace("[[[answerblock]]]", string.Empty);
                result.Add(new XElement("body", questionBody));

                List<SMAnswerContent> answerContent = stateModel.GetAnswerContent(question);

                // Randomize display order as needed
                List<long> displayMap = CreateDisplayMap(answerContent);

                XElement answerItems = new XElement("answers");

				answerItems.Add(new XAttribute("controltype", Enum.GetName(typeof(QA_AnswerControlType), question.AnswerControlType)));
				switch (question.AnswerControlType)
				{
					case QA_AnswerControlType.DATETIME:
					case QA_AnswerControlType.DATE:
					case QA_AnswerControlType.TIME:
					case QA_AnswerControlType.INTEGER:
					case QA_AnswerControlType.REAL:
					case QA_AnswerControlType.DOLLAR:
					case QA_AnswerControlType.PERCENT:
						answerItems.Add(new XAttribute("minvalue", question.ResponseMinVal));
						answerItems.Add(new XAttribute("maxvalue", question.ResponseMaxVal));
						break;
				}

                for (int i = 0; i < answerContent.Count; i++)
                {
                    int index = displayMap.IndexOf(i);
                    XElement item = new XElement("answer", answerContent[index].Content);
                    item.Add(new XAttribute("index", index));
                    answerItems.Add(item);
                }
                result.Add(answerItems);

				List<SMResponse> responses = stateModel.GetResponses(question);
				if (responses != null && responses.Count > 0)
				{
					XElement responseItems = new XElement("responses");
					for (int i = 0; i < responses.Count; i++)
					{
						int index = displayMap.IndexOf(i);
						if (index == -1) index = i;  // no map
						XElement item = new XElement("response", responses[index].Content);
						item.Add(new XAttribute("index", index));
						responseItems.Add(item);
					}
					result.Add(responseItems);
				}

                if (question.HasOtherSpecify)
                {
                    SMQuestionContent otherSpecifiedPrompt = stateModel.GetOtherSpecifiedQuestionContent(question);
                    XElement otherSpecified = new XElement("otherspecified", otherSpecifiedPrompt.Content);
                    result.Add(otherSpecified);
                }
            }
            return result.ToString();
        }

        public static string MatrixQuestionToXML(SMStateModel stateModel, SMQuestion question)
        {
            return MatrixQuestionToXML(stateModel, question, true);
        }

		public static string MatrixQuestionToXML(SMStateModel stateModel, SMQuestion question, bool evaluateContent)
		{
            // <matrixquestion>
            //    <header>header content</header>
            //    <anchors numpoints=7">
            //       <anchor>anchor content</anchor>
            //       <anchor>anchor content</anchor>
            //    </anchors>
            //    <questions>
            //       <question index="0">
            //          <body>question content</body>
            //          <response>response content</response>
            //       </question>
            //       <question index="1">
            //          <body>question content</body>
            //          <response>response content</response>
            //       </question>
            //    </questions>
            // </matrixquestion>
            //
            //  - question index is randomized as appropriate

			XElement result = new XElement("matrixquestion");

			// Header
			string questionHeader = stateModel.GetMatrixQuestionHeader(question);
            if (evaluateContent) questionHeader = stateModel.EvaluateContent(questionHeader, evaluateContent, null);
			XElement headerItem = new XElement("header", questionHeader);
			result.Add(headerItem);

			// Anchor texts
			List<SMQuestionContent> anchorTexts = stateModel.GetMatrixQuestionAnchors(question);
			int numPoints = (int)question.MatrixPointScale;
			XElement anchorItem = new XElement("anchors", questionHeader);
			anchorItem.Add(new XAttribute("numpoints", numPoints));
			for (int i = 1; i <= numPoints; i++)
			{
				string anchorText = i.ToString();
				if (anchorTexts != null && anchorTexts.Count > i - 1 && !string.IsNullOrEmpty(anchorTexts[i - 1].Content))
					anchorText = anchorTexts[i - 1].Content;
				anchorItem.Add(new XElement("anchor", anchorText));
			}
			result.Add(anchorItem);

			XElement questionsItem = new XElement("questions");
			List<SMQuestionContent> questionContents = stateModel.GetMatrixQuestionContent(question);
			List<SMResponse> responses = stateModel.GetResponses(question);
			if (questionContents != null && questionContents.Count > 0)
			{
				// Randomize display order as needed
				List<long> displayMap = CreateMatrixDisplayMap(questionContents);

				// question body rows
				for (int rowIndex = 0; rowIndex < questionContents.Count; rowIndex++)
				{
					int index = displayMap.IndexOf(rowIndex);
					string questionBody = questionContents[index].Content;
					XElement questionItem = new XElement("question");
					questionItem.Add(new XAttribute("index", index.ToString())); 
					questionItem.Add(new XElement("body", questionBody));
					if (responses != null && responses.Count > index)
					{
						questionItem.Add(new XElement("response", responses[index].Content));
					}
					else
					{
						questionItem.Add(new XElement("response"));
					}
					questionsItem.Add(questionItem);
				}
			}
			result.Add(questionsItem);

			//if (question.HasOtherSpecify)
			//{
			//    SMQuestionContent otherSpecifiedPrompt = stateModel.GetOtherSpecifiedQuestionContent(question);
			//    XElement otherSpecified = new XElement("otherspecified", otherSpecifiedPrompt.Content);
			//    result.Add(otherSpecified);
			//}
			return result.ToString();
		}

        private static List<long> CreateDisplayMap(List<SMAnswerContent> answerContent)
        {
            List<long> displayMap = new List<long>();
            for (int i = 0; i < answerContent.Count; i++)
            {
                displayMap.Add((answerContent[i].Randomize ? -1 : i));
            }
            Random random = new Random();
            for (int i = 0; i < displayMap.Count; i++)
            {
                if (!displayMap.Contains(i))
                {
                    int index = random.Next(displayMap.Count);
                    while (displayMap[index] != -1)
                        index = random.Next(displayMap.Count);
                    displayMap[index] = i;
                }
            }
            return displayMap;
        }

		private static List<long> CreateMatrixDisplayMap(List<SMQuestionContent> questionContent)
		{
			List<long> displayMap = new List<long>();
			for (int i = 0; i < questionContent.Count; i++)
			{
				displayMap.Add((questionContent[i].Randomize ? -1 : i));
			}
			Random random = new Random();
			for (int i = 0; i < displayMap.Count; i++)
			{
				if (!displayMap.Contains(i))
				{
					int index = random.Next(displayMap.Count);
					while (displayMap[index] != -1)
						index = random.Next(displayMap.Count);
					displayMap[index] = i;
				}
			}
			return displayMap;
		}
    }
}
