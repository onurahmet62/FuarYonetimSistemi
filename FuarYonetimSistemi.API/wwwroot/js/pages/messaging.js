// js/pages/messaging.js

document.addEventListener('DOMContentLoaded', async () => {
    // Bu sayfa için gerekli olan load fonksiyonu
    window.loadMessagingPage = async function() {
        console.log("Mesajlaşma sayfası yükleniyor.");
        
        // Önce kullanıcı listesini çek (yeni sohbet için)
        await window.fetchAllUsers(true);
        populateUserDropdown();
        
        setupEventListeners();
        connectToSocket();
        loadConversations();
    };

    // script.js'den doğru fonksiyonun çağrıldığından emin ol
    if (window.location.pathname.toLowerCase().includes('messaging.html')) {
        await window.loadMessagingPage();
    }
});

let socket;
let activeConversationId = null;

function connectToSocket() {
    const token = localStorage.getItem('token');
    // Socket.IO sunucunuzun adresini buraya girin.
    // Şimdilik ana API adresi olarak varsayıyoruz.
    socket = io(API_BASE_URL, {
        auth: { token }
    });

    socket.on('connect', () => {
        console.log('Socket.IO sunucusuna bağlandı. ID:', socket.id);
    });

    socket.on('newMessage', (message) => {
        console.log('Yeni mesaj alındı:', message);
        // Eğer gelen mesaj aktif sohbete aitse, direkt ekrana ekle
        if (message.conversationId === activeConversationId) {
            appendMessage(message, true);
        } else {
            // Değilse, sohbet listesindeki ilgili kişiyi bilgilendir (örn. bildirim balonu)
            const conversationItem = document.querySelector(`.list-group-item[data-id="${message.conversationId}"]`);
            if(conversationItem) {
                let unreadBadge = conversationItem.querySelector('.badge');
                if(!unreadBadge) {
                    unreadBadge = document.createElement('span');
                    unreadBadge.className = 'badge bg-danger rounded-pill ms-auto';
                    conversationItem.appendChild(unreadBadge);
                }
                unreadBadge.textContent = (parseInt(unreadBadge.textContent) || 0) + 1;
            }
        }
    });

    socket.on('connect_error', (err) => {
        console.error('Socket bağlantı hatası:', err.message);
    });
}

async function loadConversations() {
    try {
        const response = await window.fetchWithToken('/Conversations');
        const conversations = await response.json();
        const listElement = document.getElementById('conversation-list');
        listElement.innerHTML = '';
        conversations.forEach(conv => {
            const otherUser = conv.participants.find(p => p.id !== localStorage.getItem('userId')); // userId'nin saklandığını varsayıyoruz
            const userName = otherUser ? otherUser.fullName : 'Bilinmeyen Kullanıcı';
            const item = `
                <a href="#" class="list-group-item list-group-item-action" data-id="${conv.id}">
                    <div class="d-flex w-100 justify-content-between">
                        <h6 class="mb-1">${window.escapeHtml(userName)}</h6>
                        <small class="text-muted">3 gün önce</small>
                    </div>
                </a>
            `;
            listElement.innerHTML += item;
        });
    } catch (error) {
        console.error("Sohbetler yüklenemedi:", error);
    }
}

async function loadMessages(conversationId) {
    activeConversationId = conversationId;
    const chatHistory = document.getElementById('chat-history');
    chatHistory.innerHTML = '<div class="text-center p-5">Mesajlar yükleniyor...</div>';

    // Formu aktifleştir
    document.getElementById('message-to-send').disabled = false;
    document.querySelector('#message-form button').disabled = false;
    
    try {
        const response = await window.fetchWithToken(`/Conversations/${conversationId}/messages`);
        const messages = await response.json();
        chatHistory.innerHTML = '';
        messages.forEach(msg => appendMessage(msg, false));
    } catch (error) {
        chatHistory.innerHTML = '<div class="text-center text-danger p-5">Mesajlar yüklenemedi.</div>';
    }
}

function appendMessage(message, isNew) {
    const chatHistory = document.getElementById('chat-history');
    const currentUserId = localStorage.getItem('userId'); // Login'de userId saklanmalı
    const isMyMessage = message.senderId === currentUserId;
    
    const messageHtml = `
        <div class="d-flex ${isMyMessage ? 'justify-content-end' : 'justify-content-start'}">
            <div class="message ${isMyMessage ? 'my-message' : 'other-message'}">
                ${window.escapeHtml(message.content)}
                <span class="message-data-time mt-1">${window.formatDate(message.timestamp)}</span>
            </div>
        </div>
    `;
    chatHistory.insertAdjacentHTML('beforeend', messageHtml);
    if(isNew) chatHistory.scrollTop = chatHistory.scrollHeight; // Yeni mesaj gelince en alta kaydır
}

function populateUserDropdown() {
    const select = document.getElementById('selectUser');
    select.innerHTML = '<option value="">Kullanıcı seçin...</option>';
    window.AppState.users.forEach(user => {
        // Kendisi hariç diğer kullanıcıları listele
        if(user.id !== localStorage.getItem('userId')) {
             select.innerHTML += `<option value="${user.id}">${window.escapeHtml(user.fullName)}</option>`;
        }
    });
}

function setupEventListeners() {
    const newConvBtn = document.getElementById('newConversationBtn');
    const newConvModal = new bootstrap.Modal(document.getElementById('newConversationModal'));
    newConvBtn?.addEventListener('click', () => newConvModal.show());

    document.getElementById('newConversationForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const selectedUserId = document.getElementById('selectUser').value;
        if(!selectedUserId) return;
        
        // Yeni sohbet oluşturma veya mevcutu bulma logic'i backend'de olmalı
        try {
            const response = await window.fetchWithToken('/Conversations', { 
                method: 'POST', 
                body: JSON.stringify({ participantIds: [localStorage.getItem('userId'), selectedUserId] })
            });
            const newConversation = await response.json();
            newConvModal.hide();
            await loadConversations();
            await loadMessages(newConversation.id); // Yeni sohbeti direkt aç
        } catch (error) {
             Swal.fire('Hata', 'Yeni sohbet oluşturulamadı.', 'error');
        }
    });

    document.getElementById('conversation-list')?.addEventListener('click', (e) => {
        const conversationItem = e.target.closest('.list-group-item');
        if(conversationItem) {
            e.preventDefault();
            const conversationId = conversationItem.dataset.id;
            document.querySelectorAll('#conversation-list .list-group-item').forEach(el => el.classList.remove('active'));
            conversationItem.classList.add('active');
            document.getElementById('chat-with').textContent = conversationItem.querySelector('h6').textContent;
            loadMessages(conversationId);
        }
    });

    document.getElementById('message-form')?.addEventListener('submit', (e) => {
        e.preventDefault();
        const messageInput = document.getElementById('message-to-send');
        const content = messageInput.value.trim();
        if (content && activeConversationId) {
            const message = {
                conversationId: activeConversationId,
                content: content
            };
            // Mesajı hem socket üzerinden gönder hem de API'ye post et (backend logic'ine göre değişir)
            socket.emit('sendMessage', message);
            appendMessage({ ...message, senderId: localStorage.getItem('userId'), timestamp: new Date() }, true);
            messageInput.value = '';
        }
    });
}
