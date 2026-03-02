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
        
        return posts;
    }
}