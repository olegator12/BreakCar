using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Destruction
{
    public class PartHighlighter
    {
        private readonly List<IHighlightable> _glasses;
        private readonly List<IHighlightable> _wheels;
        private readonly Material _highlighted;

        public PartHighlighter(List<IHighlightable> glasses, List<IHighlightable> wheels, Material highlighted)
        {
            _glasses = glasses;
            _wheels = wheels;
            _highlighted = highlighted;
        }

        public void HighlightWheels()
        {
            Highlight(_wheels);
        }

        public void HighlightGlasses()
        {
            Highlight(_glasses);
        }

        private void Highlight(List<IHighlightable> highlightableObjects)
        {
            highlightableObjects.ForEach(item => item.Highlight(_highlighted).Forget());
        }
    }
}