using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    #region Enumerations
    public enum MGSurveyResponseType { SimpleResponse, MatrixResponse };
    #endregion

    public class MGSurveyResponse
    {
        #region Properties
        public long Id { get; set; }
        public MGSurveyResponseType TypeIdentifier { get; set; }
        #endregion

        #region Serialization
        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static MGSurveyResponse DeSerialize(string jsonStr)
        {
            JObject o = JObject.Parse(jsonStr);
            int enumValue = o["TypeIdentifier"].Value<int>();
            MGSurveyResponseType type = (MGSurveyResponseType)enumValue;
            switch (type)
            {
                case MGSurveyResponseType.SimpleResponse:
                    return MGSurveySimpleResponse.DeSerialize(jsonStr);
                case MGSurveyResponseType.MatrixResponse:
                    return MGSurveyMatrixResponse.DeSerialize(jsonStr);
            }
            return null;
        }
        #endregion
    }

    public class MGSurveySimpleResponse : MGSurveyResponse
    {
        #region Properties
        public List<string> Answers { get; set; }
        public string Others { get; set; }
        #endregion

        #region Constructor
        public MGSurveySimpleResponse()
        {
            TypeIdentifier = MGSurveyResponseType.SimpleResponse;
            Answers = new List<string>();
        }
        #endregion

        #region Serialization
        public static MGSurveySimpleResponse DeSerialize(string jsonStr)
        {
            JObject o = JObject.Parse(jsonStr);
            long id = o["Id"].Value<long>();
            int enumValue = o["TypeIdentifier"].Value<int>();
            MGSurveyResponseType type = (MGSurveyResponseType)enumValue;
            if (type == MGSurveyResponseType.SimpleResponse)
            {
                MGSurveySimpleResponse response = new MGSurveySimpleResponse() { Id = id };
				if (o["others"] != null)
				{
					response.Others = o["Others"].Value<string>();
				}

                var tokens = o["Answers"];
                if (tokens != null && tokens.Count() > 0)
                {
                    foreach (var token in tokens)
                    {
                        response.Answers.Add(token.ToString());
                    }
                }
                return response;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    public class MGSurveyMatrixResponse : MGSurveyResponse
    {
        #region Properties
        public List<List<string>> Answers { get; set; }
        public List<int> RadioAnswers { get; set; }
        #endregion

        #region Constructor
        public MGSurveyMatrixResponse()
        {
            TypeIdentifier = MGSurveyResponseType.MatrixResponse;
            Answers = new List<List<string>>();
            RadioAnswers = new List<int>();
        }
        #endregion

        #region Serialization
        public static MGSurveyMatrixResponse DeSerialize(string jsonStr)
        {
            JObject o = JObject.Parse(jsonStr);
            long id = o["Id"].Value<long>();
            int enumValue = o["TypeIdentifier"].Value<int>();
            MGSurveyResponseType type = (MGSurveyResponseType)enumValue;
            if (type == MGSurveyResponseType.MatrixResponse)
            {
                MGSurveyMatrixResponse response = new MGSurveyMatrixResponse() { Id = id };
                var tokens = o["Answers"];
                response.Answers = JsonConvert.DeserializeObject<List<List<string>>>(tokens.ToString());
                tokens = o["RadioAnswers"];
                if (tokens != null && tokens.Count() > 0)
                {
                    foreach (var token in tokens)
                    {
                        response.RadioAnswers.Add(int.Parse(token.ToString()));
                    }
                }
                return response;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}