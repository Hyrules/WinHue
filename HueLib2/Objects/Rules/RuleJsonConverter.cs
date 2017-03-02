using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2.Objects.Rules
{
    public class RuleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rule);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = (JObject)serializer.Deserialize(reader);
            Rule rule = new Rule();

            if (obj["name"] != null)
                rule.name = obj["name"].ToString();

            if (obj["owner"] != null)
                rule.owner = obj["owner"].ToObject<string>();

            if (obj["timestriggered"] != null)
                rule.timestriggered = obj["timestriggered"].ToObject<int>();

            if (obj["lasttriggered"] != null)
                rule.lasttriggered = obj["lasttriggered"].ToObject<string>();

            if (obj["created"] != null)
                rule.created = obj["created"].ToObject<string>();

            if (obj["status"] != null)
                rule.status = obj["status"].ToObject<string>();

            if(obj["actions"] != null)
            {
                rule.actions = new List<RuleAction>();
                JArray actions = obj["actions"].ToObject<JArray>();
                foreach(JToken t in actions)
                {
                    RuleAction ruleAction = new RuleAction();
                    if (t["address"] != null)
                        ruleAction.address = t["address"].ToObject<RuleAddress>();
                        
                    if(t["method"] != null)
                        ruleAction.method = t["method"].ToObject<string>();

                    if (t["body"] != null)
                    {
                        switch (ruleAction.address.objecttype)
                        {
                            case "lights":
                                ruleAction.body = t["body"].ToObject<State>();
                                break;
                            case "groups":
                                if(ruleAction.address.id == "0" && t["body"].SelectToken("scene") != null)
                                    goto case "scenes";
                                ruleAction.body = t["body"].ToObject<Action>();
                                break;
                            case "sensors":
                                if (t["body"].SelectToken("flag") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<ClipGenericFlagSensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("status") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<ClipGenericStatusState>();
                                    break;
                                }

                                if (t["body"].SelectToken("humidity") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<ClipHumiditySensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("open") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<ClipOpenCloseSensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("presence") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<PresenceSensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("lightlevel") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<LightLevelState>();
                                    break;
                                }

                                if (t["body"].SelectToken("temperature") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<TemperatureSensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("daylight") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<DaylightSensorState>();
                                    break;
                                }

                                if (t["body"].SelectToken("buttonevent") != null)
                                {
                                    ruleAction.body = t["body"].ToObject<ButtonSensorState>();
                                    break;
                                }

                                break;
                            case "scenes":
                                ruleAction.body = t["body"].ToObject<SceneBody>();
                                break;
                            case "schedules":
                                ruleAction.body = t["body"].ToObject<ScheduleBody>();
                                break;
                        }                        
                    }
                    rule.actions.Add(ruleAction);
                }

                if (obj["conditions"] != null)
                {
                    rule.conditions = new List<RuleCondition>();
                    JArray conditions = obj["conditions"].ToObject<JArray>();
                    foreach(JToken t in conditions)
                    {
                        RuleCondition ruleCondition = new RuleCondition();
                        if (t["address"] != null)
                            ruleCondition.address = t["address"].ToObject<RuleAddress>();

                        if (t["operator"] != null)
                            ruleCondition.@operator = t["operator"].ToObject<string>();

                        if (t["value"] != null)
                            ruleCondition.value = t["value"].ToObject<dynamic>();

                        rule.conditions.Add(ruleCondition);
                    }

                }
            }
            return rule;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
