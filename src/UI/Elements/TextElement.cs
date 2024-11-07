using System.IO;

using SFML.Graphics;
using SFML.System;

using Stella.Game;


namespace Stella.UI.Elements;


public class TextElement : Element
{
    public static Font DefaultTextFont = new(Path.Combine(GlobalSettings.FontsDirectory, "itim.ttf"));
    
    public Text Text { get; protected set; }
    
    public override Transformable Transformable => Text;


    public TextElement(Element? parent, Vector2f position, uint size, string text, Font? font = null) : base(parent)
    {
        Text = new(text, font ?? DefaultTextFont)
        {
            CharacterSize = size
        };
        
        Position = position;
    }


    public override void Update()
    {
        UpdateSfmlProperties();
        
        base.Update();
    }
    

    public override void Draw(RenderTarget target)
    {
        target.Draw(Text);
        
        base.Draw(target);
    }

    // TODO: use GetGlobalBounds for everything

    public override FloatRect GetBounds()
        => Text.GetGlobalBounds();


    public override Vector2f GetAlignmentPosition(AlignmentType alignment)
    {
        // text local bounds work quite different
        // https://learnsfml.com/basics/graphics/how-to-center-text/#set-a-string
        
        FloatRect localBounds = Text.GetLocalBounds();
        
        Vector2f position = base.GetAlignmentPosition(alignment);
        position -= localBounds.Position;
        
        return position;
    }
}