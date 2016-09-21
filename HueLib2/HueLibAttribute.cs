namespace HueLib2
{
    /// <summary>
    /// Custom attribute to empty properties that need to be empty at creation and modification
    /// </summary>
    public class HueLibAttribute : System.Attribute
    {
        /// <summary>
        /// Property allowed to be set at creation.
        /// </summary>
        public bool Create;

        /// <summary>
        /// Property allowed to be set at modification.
        /// </summary>
        public bool Modify;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_create">Allowed to be set at creation.</param>
        /// <param name="_modify">Allowed to be set at modification.</param>
        public HueLibAttribute(bool _create,bool _modify)
        {
            Create = _create;
            Modify = _modify;
        }
    
          
    }
}