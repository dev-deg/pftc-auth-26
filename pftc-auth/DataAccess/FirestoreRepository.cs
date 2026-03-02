using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using pftc_auth.Models;

namespace pftc_auth.DataAccess;

public class FirestoreRepository
{
    private readonly ILogger<FirestoreRepository> _logger;
    private FirestoreDb _db;

    public FirestoreRepository(ILogger<FirestoreRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _db = FirestoreDb.Create(configuration["Authentication:Google:ProjectId"]);
    }

    public async Task CreatePost(SocialMediaPost p)
    {
        await _db.Collection("posts").AddAsync(p);
        _logger.LogInformation($"Post by {p.PostAuthor} created successfully.");
    }

    public async Task<List<SocialMediaPost>> GetPosts()
    {
        List<SocialMediaPost> posts = new  List<SocialMediaPost>();

        Query allPostsQuery = _db.Collection("posts");
        QuerySnapshot querySnapshot = await allPostsQuery.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            SocialMediaPost post = documentSnapshot.ConvertTo<SocialMediaPost>();
            posts.Add(post);
        }
        _logger.LogInformation($"{posts.Count} posts found.");
        return posts;
    }
    
    public async Task<SocialMediaPost> GetPostById(string postId)
    {
        Query allPostsQuery = _db.Collection("posts").WhereEqualTo("postId", postId);
        QuerySnapshot querySnapshot = await allPostsQuery.GetSnapshotAsync();

        if (querySnapshot.Documents.Count == 0)
            throw new KeyNotFoundException($"Post with id {postId} not found");
        
        DocumentSnapshot documentSnapshot = querySnapshot.Documents[0];
        SocialMediaPost post = documentSnapshot.ConvertTo<SocialMediaPost>();
        
        _logger.LogInformation($"Returning post {postId} by {post.PostAuthor}");
        return post;
    }

    public async Task DeletePost(string postId)
    {
        if (string.IsNullOrWhiteSpace(postId))
        {
            throw new ArgumentException("Post ID cannot be null or empty.", nameof(postId));
        }

        try
        {
            //Load the post
            Query allPostsQuery = _db.Collection("posts").WhereEqualTo("postId", postId);
            QuerySnapshot querySnapshot = await allPostsQuery.GetSnapshotAsync();

            if (querySnapshot.Documents.Count == 0)
                throw new KeyNotFoundException($"Post with id {postId} not found");
        
            DocumentSnapshot documentSnapshot = querySnapshot.Documents[0];
            
            //Delete it
            await documentSnapshot.Reference.DeleteAsync();
            _logger.LogInformation($"Post with id {postId} deleted successfully.");
            //If later on we have multimedia (images/videos) these need to be deleted aswell
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
        catch (Exception e) when (e is not ArgumentException)
        {
            _logger.LogError(e, $"Unexpeted error deleting post by {postId}");
            throw;
        }
        
    }
}