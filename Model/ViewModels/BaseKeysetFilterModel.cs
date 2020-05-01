using System;
using ChatServer.Model.Enum;

namespace ChatServer.Model.ViewModels {
    public abstract class BaseKeysetFilterModel {
        public int Limit { get; set; }
        public DateTime EdgeDateTime { get; set; }
        public KeysetFilterModelType DirectionType { get; set; }

        public BaseKeysetFilterModel () {
            this.Limit = 2;
            EdgeDateTime = DateTime.Now;
            DirectionType=KeysetFilterModelType.Previous;
        }
    }
}