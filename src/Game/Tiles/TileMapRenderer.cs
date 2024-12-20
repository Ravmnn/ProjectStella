using SFML.System;
using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileMapRenderer
{
    private readonly object _renderLock = new();
    
    private Tile[,] _tiles;
    public Tile[,] Tiles
    {
        get => _tiles;
        set
        {
            _tiles = value;
            TileVertices = VertexArrayFromTiles(value);
        }
    }
    
    public TileSet TileSet { get; }
    private readonly Texture _tileSetTexture;
    
    public VertexArray TileVertices { get; private set; }
    public bool NoTextures { get; set; }
    

    public TileMapRenderer(Tile[,] tiles)
    {
        _tiles = tiles;
        TileVertices = VertexArrayFromTiles(tiles);
        
        TileSet = TileSet.Global;
        _tileSetTexture = new(TileSet);
    }
    

    public void Render(RenderTarget target)
    {
        lock (_renderLock)
        {
            if (NoTextures)
                target.Draw(TileVertices);
            else
                target.Draw(TileVertices, new(_tileSetTexture));
        }
    }


    public void UpdateVertices()
    {
        uint currentIndex = 0;

        foreach (Tile tile in Tiles)
        {
            if (tile.Object is not null)
                UpdateVerticesOfTile(tile.Object, currentIndex);
                                
            // we are using triangles to represent tiles, so we got 2 triangles, resulting in 6 vertices
            currentIndex += 6;
        }
    }


    private void UpdateVerticesOfTile(TileDrawable tile, uint currentIndex)
    {
        FloatRect bounds = tile.GetGlobalBounds();
        
        float x = tile.Position.X;
        float y = tile.Position.Y;
        float sx = bounds.Width;
        float sy = bounds.Height;
        
        lock (_renderLock)
        {
            if (NoTextures)
            {
                Color color = TileIndex.LoadedTiles[tile.Name].Color;
            
                TileVertices[currentIndex + 0] = new(new(x, y), color);
                TileVertices[currentIndex + 1] = new(new(x + sx, y), color);
                TileVertices[currentIndex + 2] = new(new(x, y + sy), color);
                TileVertices[currentIndex + 3] = new(new(x, y + sy), color);
                TileVertices[currentIndex + 4] = new(new(x + sx, y + sy), color);
                TileVertices[currentIndex + 5] = new(new(x + sx, y), color);
            }
            else
            {
                Vector2i texturePosition = TileSet.GetTilePixelPositionFromIndex(tile.Index);
        
                float tx = texturePosition.X;
                float ty = texturePosition.Y;
                float ts = TileSet.TileSize; // symmetric
        
                TileVertices[currentIndex + 0] = new(new(x, y), new Vector2f(tx, ty));
                TileVertices[currentIndex + 1] = new(new(x + sx, y), new Vector2f(tx + ts, ty));
                TileVertices[currentIndex + 2] = new(new(x, y + sy), new Vector2f(tx, ty + ts));
                TileVertices[currentIndex + 3] = new(new(x, y + sy), new Vector2f(tx, ty + ts));
                TileVertices[currentIndex + 4] = new(new(x + sx, y + sy), new Vector2f(tx + ts, ty + ts));
                TileVertices[currentIndex + 5] = new(new(x + sx, y), new Vector2f(tx + ts, ty));
            }
        }
    }
    
    
    private static VertexArray VertexArrayFromTiles(Tile[,] tiles)
        => new(PrimitiveType.Triangles, (uint)(6 * tiles.GetLength(1) * tiles.GetLength(0)));
}