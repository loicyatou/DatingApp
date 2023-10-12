
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext _context, IMapper mapper)
    {
        this._context = _context;
        _mapper = mapper;
    }
    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await _context.Messages
        .Include(u => u.Sender).ThenInclude(p => p.Photos)
        .Include(u => u.Recipient).ThenInclude(p => p.Photos)

        //where condtion filters the messages from context on two conditions: that both the sender and receiver are either the senders or recievers of those messages.
        .Where(
            m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
            m.SenderUsername == recipientUserName || //This condition matches messages where the current user is the recipient and the other user is the sender.
            m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
            m.SenderUsername == currentUserName //This condition matches messages where the current user is the sender and the other user is the recipient.
        )
        .OrderBy(m => m.MessageSent) //order by time of messages so its organised logically
        .ToListAsync(); //creates a list of those messages in memory

        //messages that were previously unread now need to be marked as read if this method is triggered
        var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName).ToList(); //dont need to get this from DB since its in memory from the list above

        if (unreadMessages.Any())
        {
            foreach (var m in unreadMessages)
            {
                m.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        //creates query that takes all instances from data context in messages table --> this is LINQ query
        var query = _context.Messages
        .OrderByDescending(x => x.MessageSent)
        .AsQueryable(); //makes them queryable --> so that you can apply rules to the query as follows

        //instances returned will depend on the container messageParams contains.
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
            && u.RecipientDeleted == false), //inbox will return all messages that have been sent to the current user

            "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
            && u.SenderDeleted == false), //outbox returns all messages sent by the user

            _ => query.Where(u => u.RecipientUsername == messageParams.Username
            && u.RecipientDeleted == false && u.DateRead == null) //default to the messageParams default which is the unread messages.
        };

        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize); //messages might be numerous pages so it will map them out across numerous pages.
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
