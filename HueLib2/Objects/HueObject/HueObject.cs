using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace HueLib2
{
    /// <summary>
    /// Abstract class for every hue object Light, Group, Sensor, Schedule, Scene.
    /// </summary>
    [DataContract]
    public abstract class HueObject : INotifyPropertyChanged
    {

       
        /// <summary>
        /// Image of the object.
        /// </summary>
        ImageSource _image;

        /// <summary>
        /// ID of the item.
        /// </summary>
        string _id;

        #region PROPERTIES

        /// <summary>
        /// Image accessor.
        /// </summary>
        [Browsable(false), HueLib(false, false)]
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Id accessor.
        /// </summary>
        [Browsable(false), HueLib(false, false)]
        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        #endregion

        #region EVENTS
        /// <summary>
        /// Event that happen when property has change.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When a property has change this event is triggered - needed for the binding to refresh properly.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
