using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_TriggerUserEventLog")]
    public class TriggerUserEventLog : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long MemberId { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string EventName { get; set; }

        [PetaPoco.Column(Length = 10, IsNullable = false)]
		public string Channel { get; set; }

        [PetaPoco.Column]
		public string Context { get; set; }

        [PetaPoco.Column]
		public string RulesExecuted { get; set; }

        [PetaPoco.Column]
		public string Result { get; set; }		

        public string SerializeContext(Dictionary<string, object> contextMap)
        {
            string xml = null;

            if (contextMap != null && contextMap.Count > 0)
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("TriggerUserEventContext");
                doc.Add(root);

                foreach (KeyValuePair<string, object> kv in contextMap)
                {
                    if (kv.Key != "Result")
                    {
                        XElement node = new XElement("Context");
                        XAttribute att = new XAttribute("Name", kv.Key);
                        node.Add(att);
                        att = new XAttribute("Value", kv.Value.ToString());
                        node.Add(att);
                        root.Add(node);
                    }
                }

                xml = doc.ToString();
            }

            return xml;
        }

        public string SerailizeRuleExecutionLogIds(IList<ContextObject.RuleResult> results)
        {
            string xml = null;

            if (results != null && results.Count > 0)
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("TriggerUserEventRuleXecutionLogs");
                doc.Add(root);

                foreach (ContextObject.RuleResult result in results)
                {
                    if (result.ExecutionLogId > 0)
                    {
                        XElement node = new XElement("RuleExecutionLog");
                        XAttribute att = new XAttribute("Id", result.ExecutionLogId);
                        node.Add(att);
                        root.Add(node);
                    }
                }

                xml = doc.ToString();
            }

            return xml;
        }
    }

    
}
