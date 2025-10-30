namespace Poki.Shared;

public static class ResourcePaths
{
    private static readonly string Resources = Path.Combine(SharedUtils.TryGetSolutionDirectory() ?? ".", "resources");
    
    /// <summary>
    /// Pokemon data
    /// </summary>
    public static readonly string PokiData = Path.Combine(Resources, "poki-data");
    
    /// <summary>
    /// Poki-related images, also hosted on https://poki420.gitlab.io/poki-resources
    /// </summary>
    public static readonly string PokiSprites = Path.Combine(Resources, "poki-sprites", "public");

    public static readonly string PokeApiSprites = Path.Combine(Resources, "pokeapi-sprites", "sprites");
    
    public static readonly string PokiBoxIcons = Path.Combine(Resources, "poki-box-icons");
    public static readonly string Pokesprite = Path.Combine(Resources, "pokesprite");
}