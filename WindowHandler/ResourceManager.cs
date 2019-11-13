using System.Collections.Generic;

namespace MLLib.WindowHandler
{
    public class ResourceManager
    {
        private Dictionary<int, Texture> _textures;
        private List<StringRenderer> _stringRenderers;

        public ResourceManager()
        {
            _textures = new Dictionary<int, Texture>();
            _stringRenderers = new List<StringRenderer>();
        }

        public int TexturesCount => _textures.Count;

        public void PushRenderer(StringRenderer renderer)
        {
            _stringRenderers.Add(renderer);
        }

        public void PushTexture(Texture tex)
        {
            _textures.Add(_textures.Count, tex);
        }

        public void PushTexture(int id, Texture tex)
        {
            _textures.Add(id, tex);
        }

        public void PushTexture(int id, string filename)
        {
            _textures.Add(id, new Texture(filename));
        }

        public void FreeAll()
        {
            foreach (var pair in _textures)
                pair.Value.Dispose();

            foreach (var renderer in _stringRenderers)
                renderer._renderer.Dispose();
        }

        public Texture Get(int texture)
        {
            return _textures[texture];
        }
    }
}