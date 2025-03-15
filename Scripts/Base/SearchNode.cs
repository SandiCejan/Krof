using Microsoft.Xna.Framework;

namespace KrofEngine
{
    public class SearchNode
    {
        protected Point _point;
        protected SearchNode _parent;
        protected int _realCost, _heuristicCost;
        public int TotalCost => _realCost + _heuristicCost;

        public static SearchNode Node()
        {
            return new SearchNode();
        }

        public Point Point
        {
            get => _point;
            set => _point = value;
        }

        public SearchNode Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public int RealCost
        {
            get => _realCost;
            set => _realCost = value;
        }

        public int HeuristicCost
        {
            get => _heuristicCost;
            set => _heuristicCost = value;
        }
    }
}
