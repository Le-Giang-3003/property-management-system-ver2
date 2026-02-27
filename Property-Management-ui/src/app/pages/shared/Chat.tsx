import { useState } from "react";
import { Search, Send, Phone, Video, MoreVertical, Paperclip, Smile } from "lucide-react";

interface Message {
  id: string;
  sender: string;
  content: string;
  timestamp: string;
  isCurrentUser: boolean;
}

interface ChatUser {
  id: string;
  name: string;
  avatar: string;
  lastMessage: string;
  timestamp: string;
  online: boolean;
  unread?: number;
}

const chatUsers: ChatUser[] = [
  {
    id: "1",
    name: "Căn hộ Vinhomes Central Park",
    avatar: "TH",
    lastMessage: "Phạm Thị Hoa",
    timestamp: "2 phút",
    online: true,
  },
  {
    id: "2",
    name: "Nhà phố Thảo Điền",
    avatar: "TD",
    lastMessage: "Phạm Thị Hoa",
    timestamp: "15 phút",
    online: false,
  },
  {
    id: "3",
    name: "Căn hộ Vinhomes Central Park",
    avatar: "VC",
    lastMessage: "Phạm Thị Hoa",
    timestamp: "1 giờ",
    online: true,
    unread: 2,
  },
];

const messages: Message[] = [
  {
    id: "1",
    sender: "Phạm Thị Hoa",
    content: "Chào anh, em muốn hỏi về căn hộ này ạ",
    timestamp: "10:30 AM",
    isCurrentUser: false,
  },
  {
    id: "2",
    sender: "Bạn",
    content: "Chào em, anh có thể giúp gì cho em?",
    timestamp: "10:32 AM",
    isCurrentUser: true,
  },
  {
    id: "3",
    sender: "Phạm Thị Hoa",
    content: "Em có thể xem căn hộ vào cuối tuần này được không ạ?",
    timestamp: "10:33 AM",
    isCurrentUser: false,
  },
  {
    id: "4",
    sender: "Bạn",
    content: "Được chứ em. Em muốn xem vào thời gian nào?",
    timestamp: "10:35 AM",
    isCurrentUser: true,
  },
];

export default function Chat() {
  const [selectedChat, setSelectedChat] = useState<ChatUser>(chatUsers[0]);
  const [messageInput, setMessageInput] = useState("");
  const [searchQuery, setSearchQuery] = useState("");

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Tin nhắn</div>
        <div className="page-desc">Trao đổi với chủ nhà và người thuê</div>
      </div>

      <div
        className="card"
        style={{
          padding: 0,
          overflow: "hidden",
          height: "calc(100vh - 240px)",
          display: "flex",
        }}
      >
        {/* Chat List Sidebar */}
        <div
          style={{
            width: 320,
            borderRight: "1px solid var(--border)",
            display: "flex",
            flexDirection: "column",
          }}
        >
          {/* Search */}
          <div style={{ padding: 16, borderBottom: "1px solid var(--border)" }}>
            <div className="admin-search-wrapper" style={{ margin: 0 }}>
              <Search size={18} className="admin-search-icon" />
              <input
                className="admin-search-input"
                placeholder="Tìm kiếm..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                style={{ padding: "8px 12px 8px 36px" }}
              />
            </div>
          </div>

          {/* Chat List */}
          <div style={{ flex: 1, overflowY: "auto" }}>
            {chatUsers.map((user) => (
              <div
                key={user.id}
                onClick={() => setSelectedChat(user)}
                style={{
                  padding: 16,
                  borderBottom: "1px solid var(--border)",
                  cursor: "pointer",
                  background:
                    selectedChat.id === user.id
                      ? "var(--bg-hover)"
                      : "transparent",
                  transition: "background 0.2s",
                }}
                onMouseEnter={(e) => {
                  if (selectedChat.id !== user.id) {
                    e.currentTarget.style.background = "rgba(255,255,255,0.02)";
                  }
                }}
                onMouseLeave={(e) => {
                  if (selectedChat.id !== user.id) {
                    e.currentTarget.style.background = "transparent";
                  }
                }}
              >
                <div style={{ display: "flex", gap: 12, alignItems: "center" }}>
                  <div style={{ position: "relative" }}>
                    <div
                      style={{
                        width: 48,
                        height: 48,
                        borderRadius: "50%",
                        background: "var(--accent-light)",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        color: "white",
                        fontWeight: 600,
                        fontSize: 16,
                      }}
                    >
                      {user.avatar}
                    </div>
                    {user.online && (
                      <div
                        style={{
                          position: "absolute",
                          bottom: 2,
                          right: 2,
                          width: 12,
                          height: 12,
                          borderRadius: "50%",
                          background: "#10b981",
                          border: "2px solid var(--bg-card)",
                        }}
                      />
                    )}
                  </div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div
                      style={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        marginBottom: 4,
                      }}
                    >
                      <div
                        style={{
                          fontSize: 14,
                          fontWeight: 600,
                          color: "var(--text-primary)",
                          overflow: "hidden",
                          textOverflow: "ellipsis",
                          whiteSpace: "nowrap",
                        }}
                      >
                        {user.name}
                      </div>
                      {user.unread && (
                        <span
                          className="badge badge-danger"
                          style={{
                            minWidth: 20,
                            height: 20,
                            borderRadius: 10,
                            fontSize: 10,
                            padding: "0 6px",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                          }}
                        >
                          {user.unread}
                        </span>
                      )}
                    </div>
                    <div
                      style={{
                        display: "flex",
                        justifyContent: "space-between",
                        fontSize: 12,
                        color: "var(--text-muted)",
                      }}
                    >
                      <span
                        style={{
                          overflow: "hidden",
                          textOverflow: "ellipsis",
                          whiteSpace: "nowrap",
                        }}
                      >
                        {user.lastMessage}
                      </span>
                      <span style={{ marginLeft: 8, flexShrink: 0 }}>
                        {user.timestamp}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Chat Area */}
        <div style={{ flex: 1, display: "flex", flexDirection: "column" }}>
          {/* Chat Header */}
          <div
            style={{
              padding: 16,
              borderBottom: "1px solid var(--border)",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <div style={{ display: "flex", gap: 12, alignItems: "center" }}>
              <div style={{ position: "relative" }}>
                <div
                  style={{
                    width: 40,
                    height: 40,
                    borderRadius: "50%",
                    background: "var(--accent-light)",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    color: "white",
                    fontWeight: 600,
                  }}
                >
                  {selectedChat.avatar}
                </div>
                {selectedChat.online && (
                  <div
                    style={{
                      position: "absolute",
                      bottom: 0,
                      right: 0,
                      width: 10,
                      height: 10,
                      borderRadius: "50%",
                      background: "#10b981",
                      border: "2px solid var(--bg-card)",
                    }}
                  />
                )}
              </div>
              <div>
                <div
                  style={{
                    fontSize: 14,
                    fontWeight: 600,
                    color: "var(--text-primary)",
                  }}
                >
                  {selectedChat.name}
                </div>
                {selectedChat.online && (
                  <div
                    style={{
                      fontSize: 11,
                      color: "#10b981",
                      fontWeight: 500,
                    }}
                  >
                    ● Online
                  </div>
                )}
              </div>
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button
                className="btn-icon"
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "none",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  cursor: "pointer",
                  color: "var(--text-muted)",
                }}
              >
                <Phone size={18} />
              </button>
              <button
                className="btn-icon"
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "none",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  cursor: "pointer",
                  color: "var(--text-muted)",
                }}
              >
                <Video size={18} />
              </button>
              <button
                className="btn-icon"
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "none",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  cursor: "pointer",
                  color: "var(--text-muted)",
                }}
              >
                <MoreVertical size={18} />
              </button>
            </div>
          </div>

          {/* Messages */}
          <div
            style={{
              flex: 1,
              overflowY: "auto",
              padding: 20,
              display: "flex",
              flexDirection: "column",
              gap: 16,
            }}
          >
            {messages.map((msg) => (
              <div
                key={msg.id}
                style={{
                  display: "flex",
                  justifyContent: msg.isCurrentUser ? "flex-end" : "flex-start",
                }}
              >
                <div
                  style={{
                    maxWidth: "60%",
                    display: "flex",
                    flexDirection: "column",
                    alignItems: msg.isCurrentUser ? "flex-end" : "flex-start",
                  }}
                >
                  {!msg.isCurrentUser && (
                    <div
                      style={{
                        fontSize: 11,
                        color: "var(--text-muted)",
                        marginBottom: 4,
                        marginLeft: 12,
                      }}
                    >
                      {msg.sender}
                    </div>
                  )}
                  <div
                    style={{
                      padding: "10px 16px",
                      borderRadius: 12,
                      background: msg.isCurrentUser
                        ? "var(--accent-light)"
                        : "var(--bg-hover)",
                      color: msg.isCurrentUser ? "white" : "var(--text-primary)",
                      fontSize: 13,
                      lineHeight: 1.5,
                    }}
                  >
                    {msg.content}
                  </div>
                  <div
                    style={{
                      fontSize: 10,
                      color: "var(--text-muted)",
                      marginTop: 4,
                      marginLeft: msg.isCurrentUser ? 0 : 12,
                      marginRight: msg.isCurrentUser ? 12 : 0,
                    }}
                  >
                    {msg.timestamp}
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Message Input */}
          <div
            style={{
              padding: 16,
              borderTop: "1px solid var(--border)",
            }}
          >
            <div
              style={{
                display: "flex",
                gap: 8,
                alignItems: "center",
              }}
            >
              <button
                className="btn-icon"
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "none",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  cursor: "pointer",
                  color: "var(--text-muted)",
                }}
              >
                <Paperclip size={18} />
              </button>
              <button
                className="btn-icon"
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "none",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  cursor: "pointer",
                  color: "var(--text-muted)",
                }}
              >
                <Smile size={18} />
              </button>
              <input
                type="text"
                placeholder="Nhập tin nhắn..."
                value={messageInput}
                onChange={(e) => setMessageInput(e.target.value)}
                style={{
                  flex: 1,
                  padding: "10px 16px",
                  borderRadius: 8,
                  background: "var(--bg-hover)",
                  border: "1px solid var(--border)",
                  color: "var(--text-primary)",
                  fontSize: 13,
                  outline: "none",
                }}
                onKeyPress={(e) => {
                  if (e.key === "Enter" && messageInput.trim()) {
                    // Handle send message
                    setMessageInput("");
                  }
                }}
              />
              <button
                className="btn btn-primary"
                disabled={!messageInput.trim()}
                style={{
                  width: 36,
                  height: 36,
                  borderRadius: 8,
                  padding: 0,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  opacity: messageInput.trim() ? 1 : 0.5,
                }}
              >
                <Send size={18} />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
