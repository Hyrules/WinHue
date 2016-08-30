namespace HueLib2
{
    /// <summary>
    /// ActionBody class.
    /// </summary>
    public class SceneBody : RuleBody
    {
        /// <summary>
        /// Scene variable.
        /// </summary>
        public string scene { get; set; }

        /// <summary>
        /// Override to string function.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return scene;
        }
    }
}
