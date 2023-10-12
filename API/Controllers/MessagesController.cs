using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername(); //get user signed in from ticket

        if (username == createMessageDTO.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself"); //if the username you get is the same as the username put onto the createMessage component then lock it off

        var sender = await _userRepository.GetUserByUsernameAsync(username); //return appuser instance of sender
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername); //get user instance of recipient

        if (recipient == null) return NotFound(); //if the recipient does not exist then return null

        var message = new Message //create message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        _messageRepository.AddMessage(message); //store message in datacontext 

        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message)); //save to data context and return json of message to client

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        //By adding the pagination header to the response using `Response.AddPaginationHeader`, the server is informing the client about the pagination details. This allows the client to understand the current page, the total number of pages, and other relevant information for displaying and navigating through the messages.Including this information in the header is beneficial because it separates the metadata from the actual message data, making it easier for the client to parse and utilize the pagination information. It also follows the standard practice of using headers for providing additional context and metadata in API responses

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
        messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<MessageDTO>> GetMessageThread(string username)
    {
        var currentUserName = User.GetUsername();

        return Ok(await _messageRepository.GetMessageThread(currentUserName, username));

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _messageRepository.GetMessage(id);

        if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized(); //if neither the sender or reciever are trying to access the record then prevent access

        if (message.SenderUsername == username) message.SenderDeleted = true; //You need both to be true in order to remove the data from the database otherwise it will remain for the other person
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted) //if both true then remove the message from the database
        {
            _messageRepository.DeleteMessage(message);
        }

        if(await _messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting the message");
    }

}
