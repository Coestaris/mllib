using System.Collections.Generic;

namespace WindowHandler
{
    public class ResourceManager
    {
        private Dictionary<int, Texture> _textures;

        public ResourceManager()
        {
            _textures = new Dictionary<int, Texture>();
        }

        public void PushTexture(int id, Texture tex)
        {
            _textures.Add(id, tex);
        }

        public void PushTexture(int id, string filename)
        {
            _textures.Add(id, new Texture(filename));
        }

        public void FreeTexture(int id)
        {
            _textures[id].Dispose();
        }

        public void FreeAll()
        {
            foreach (var pair in _textures)
                pair.Value.Dispose();
        }

        public Texture Get(int texture)
        {
            return _textures[texture];
        }
    }
}