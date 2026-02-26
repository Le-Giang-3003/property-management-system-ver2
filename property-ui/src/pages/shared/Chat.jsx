import { useState } from 'react';
import { conversations, messages } from '../../data/mockData';
import { Send, Paperclip, Search } from 'lucide-react';

export default function Chat() {
  const [activeChatId, setActiveChatId] = useState(1);
  const [newMessage, setNewMessage] = useState('');
  const [localMessages, setLocalMessages] = useState(messages);

  const activeChat = conversations.find(c => c.id === activeChatId);
  const chatMessages = localMessages.filter(m => m.conversationId === activeChatId);

  const sendMessage = () => {
    if (!newMessage.trim()) return;
    const msg = { id: Date.now(), conversationId: activeChatId, senderId: 1, senderName: 'Tôi', messageType: 'Text', content: newMessage, isRead: true, isEdited: false, isDeleted: false, sentAt: new Date().toISOString() };
    setLocalMessages(prev => [...prev, msg]);
    setNewMessage('');
  };

  const formatTime = (dateStr) => {
    try { return new Date(dateStr).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }); }
    catch { return ''; }
  };

  return (
    <div style={{ height: 'calc(100vh - 100px)' }}>
      <div className="mb-16">
        <div className="page-title">Tin nhắn</div>
        <div className="page-desc">Trao đổi với chủ nhà và người thuê</div>
      </div>

      <div className="chat-layout" style={{ height: 'calc(100% - 80px)' }}>
        {/* Conversation List */}
        <div className="chat-list">
          <div className="chat-list-header">
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <span>💬 Hội thoại</span>
              <span className="badge badge-danger">{conversations.reduce((s, c) => s + c.unreadCount, 0)}</span>
            </div>
            <div className="header-search" style={{ marginTop: 10 }}>
              <Search size={13} style={{ color: 'var(--text-muted)' }} />
              <input placeholder="Tìm kiếm..." style={{ background: 'transparent', border: 'none', outline: 'none', color: 'var(--text-primary)', fontSize: 12, width: '100%' }} />
            </div>
          </div>
          {conversations.map(c => (
            <div key={c.id} className={`chat-item ${activeChatId === c.id ? 'active' : ''}`} onClick={() => setActiveChatId(c.id)}>
              <div className="chat-avatar">{c.otherUserName[0]}</div>
              <div className="chat-meta">
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <div className="chat-name">{c.otherUserName}</div>
                  <div className="chat-time">
                    {new Date(c.lastMessageAt).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })}
                  </div>
                </div>
                {c.propertyTitle && <div style={{ fontSize: 10, color: 'var(--accent-light)', marginBottom: 2 }}>🏠 {c.propertyTitle}</div>}
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <div className="chat-preview">{c.lastMessageContent}</div>
                  {c.unreadCount > 0 && <div className="chat-unread">{c.unreadCount}</div>}
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Chat Window */}
        <div className="chat-window">
          {activeChat && (
            <>
              <div className="chat-header">
                <div className="chat-avatar" style={{ width: 40, height: 40 }}>{activeChat.otherUserName[0]}</div>
                <div>
                  <div style={{ fontWeight: 600, fontSize: 14 }}>{activeChat.otherUserName}</div>
                  {activeChat.propertyTitle && <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>🏠 {activeChat.propertyTitle}</div>}
                </div>
                <div style={{ marginLeft: 'auto' }}>
                  <span className="badge badge-success">● Online</span>
                </div>
              </div>

              <div className="chat-messages">
                {chatMessages.map(msg => {
                  const isMe = msg.senderId === 1 || msg.senderId === 2; // Landlord (id=2) is "me" for landlord view
                  return (
                    <div key={msg.id} className={`message ${isMe ? 'mine' : ''}`}>
                      {!isMe && (
                        <div className="chat-avatar" style={{ width: 30, height: 30, fontSize: 11 }}>{msg.senderName[0]}</div>
                      )}
                      <div>
                        {!isMe && <div style={{ fontSize: 11, color: 'var(--text-muted)', marginBottom: 4 }}>{msg.senderName}</div>}
                        <div className={`message-bubble ${isMe ? 'mine' : 'theirs'}`}>{msg.content}</div>
                        <div className="message-time">{formatTime(msg.sentAt)}</div>
                      </div>
                    </div>
                  );
                })}
                {chatMessages.length === 0 && (
                  <div className="empty-state"><div className="empty-icon">💬</div><p>Chưa có tin nhắn nào. Hãy bắt đầu cuộc trò chuyện!</p></div>
                )}
              </div>

              <div className="chat-input-area">
                <button className="btn btn-ghost btn-sm btn-icon"><Paperclip size={16}/></button>
                <input
                  className="chat-input"
                  placeholder="Nhập tin nhắn..."
                  value={newMessage}
                  onChange={e => setNewMessage(e.target.value)}
                  onKeyDown={e => e.key === 'Enter' && !e.shiftKey && (e.preventDefault(), sendMessage())}
                />
                <button className="btn btn-primary btn-sm btn-icon" onClick={sendMessage}>
                  <Send size={16}/>
                </button>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
