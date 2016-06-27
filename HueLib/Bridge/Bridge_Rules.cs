using System;
using System.Collections.Generic;
using System.Linq;
using HueLib_base;

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
            Dictionary<string, Rule> listRules;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    listRules = Serializer.DeserializeToObject<Dictionary<string,Rule>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    listRules = null;
                    
                }
            }
            catch(Exception)
            {
                if(!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));

                listRules = null;
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    rule = Serializer.DeserializeToObject<Rule>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    rule = null;
                }
            }
            catch(Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                rule = null;
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
            string id = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules"), WebRequestType.POST, Serializer.SerializeToJson<Rule>(newRule));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        id = ((CreationSuccess) lastMessages[0]).id;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
 
                }
            }
            catch (Exception)
            {
                id = null;             
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
            string result = null;

            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Rule>(modifiedRule));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount > 0)
                        result = ((Success) lastMessages[0]).id;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = null;
          
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.DELETE);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        result = true;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;

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

            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/rules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Rule>(new Rule() {name = newName}));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }

            return lastMessages;

        }
    }
}
