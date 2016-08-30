using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HueLib.BridgeMessages.Error;
using HueLib2;

namespace HueLib
{
    public partial class Bridge
    {
        /// <summary>
        /// Get the list of rules present in the bridge.
        /// </summary>
        /// <param name="sensorId">If specified the list will return only those related to the desired sensor.</param>
        /// <returns>A list of rules.</returns>
        public Dictionary<string,Rule> GetRulesList()
        {
            Dictionary<string, Rule> listRules = new Dictionary<string, Rule>();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    listRules = Serializer.DeserializeToObject<Dictionary<string, Rule>>(comres.data);
                    if (listRules != null) return listRules;
                    listRules = new Dictionary<string, Rule>();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres) } : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return listRules;
        }

        /// <summary>
        /// Return the desired rule from the bridge.
        /// </summary>
        /// <param name="id">ID of the desired rule</param>
        /// <returns>A rule from the bridge.</returns>
        public Rule GetRule(string id)
        {
            Rule rule = new Rule();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    rule = Serializer.DeserializeToObject<Rule>(comres.data);
                    if (rule != null) return rule;
                    rule = new Rule();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres) } : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return rule;
        }

        /// <summary>
        /// Create a new rule on the bridge.
        /// </summary>
        /// <param name="newRule">The new rule to be created.</param>
        /// <returns>The ID of the created rule if succesful.</returns>
        public string CreateRule(Rule newRule)
        {
            string id = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules"), WebRequestType.POST, Serializer.SerializeToJson<Rule>(newRule));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            id = ((CreationSuccess) lastMessages[0]).id;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return id;
        }

        /// <summary>
        /// Modify an already existing rule.
        /// </summary>
        /// <param name="id">ID of the rule to be modified.</param>
        /// <param name="modifiedRule">The desired modification to the rule.</param>
        /// <returns>The id of the modified rule if successful</returns>
        public string ModifyRule(string id, Rule modifiedRule)
        {
            string result = "";

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Rule>(modifiedRule));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount > 0)
                            result = ((Success)lastMessages[0]).id;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return result;
        }

        /// <summary>
        /// Delete the specified rule.
        /// </summary>
        /// <param name="id">ID of the rule to be deleted.</param>
        /// <returns>True or False the rule has been deleted.</returns>
        public bool DeleteRule(string id)
        {
            bool result = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.DELETE);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            result = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return result;
        }

        /// <summary>
        /// Change the name of a Rule.
        /// </summary>
        /// <param name="id">ID of the rule to change the name.</param>
        /// <param name="newName">New name of the rule.</param>
        /// <returns>A message collection.</returns>
        public MessageCollection ChangeRuleName(string id, string newName)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Rule>(new Rule() { name = newName }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                    }                      
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return lastMessages;

        }
    }
}
