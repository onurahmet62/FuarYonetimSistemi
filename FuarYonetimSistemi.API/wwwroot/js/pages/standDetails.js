// dosya: js/pages/standDetails.js

/**
 * Stand detay sayfasını başlatan ana fonksiyon.
 */
window.loadStandDetailsPage = async function (standId) {
    if (!standId) {
        Swal.fire('Hata', 'Stand ID bulunamadı.', 'error').then(() => window.location.href = 'liststand.html');
        return;
    }

    try {
        // Gerekli tüm verileri önbelleğe al veya yenile
        await Promise.all([
            window.fetchAllFairs(false),
            window.fetchAllParticipants(false)
        ]);

        // Bu standa özel verileri çek
        const [stand, paymentsResult] = await Promise.all([
            window.fetchWithToken(`/Stands/${standId}`),
            window.fetchWithToken(`/Payments/filter`, {
                method: 'POST',
                body: JSON.stringify({ standId: standId, pageSize: 1000 })
            })
        ]);

        const allPaymentsForStand = window.processApiResponse(paymentsResult);

        // GÜVENLİK FİLTRESİ: API'den gelen verinin kesinlikle bu standa ait olduğundan emin ol.
        const payments = allPaymentsForStand.filter(p => p.standId === standId);

        // İlgili fuar ve katılımcıyı bul
        const fair = window.AppState.fairs.find(f => f.id === stand.fairId);
        const participant = window.AppState.participants.find(p => p.id === stand.participantId);

        // Hesaplamaları yap
        const totalAmount = stand.totalAmountWithVAT || ((stand.contractArea || 0) * (stand.unitPrice || 0));
        const totalPaid = payments.reduce((sum, p) => sum + (p.amount || 0), 0);
        const balance = totalAmount - totalPaid;

        // Sayfayı doldur
        populatePage(stand, fair, participant, payments, { totalAmount, totalPaid, balance });
        setupEventListeners(stand, participant);

    } catch (error) {
        console.error("Stand detayları yüklenirken kritik bir hata oluştu:", error);
        Swal.fire('Hata!', error.message || 'Stand bilgileri yüklenirken bir sorun oluştu.', 'error');
    }
};

/**
 * Sayfadaki tüm UI elementlerini gelen ve hesaplanan verilerle doldurur.
 */
function populatePage(stand, fair, participant, payments, financial) {
    document.getElementById('standNoTitle').textContent = window.escapeHtml(stand.name || stand.standNo);
    document.title = `Ardesk - Stand: ${window.escapeHtml(stand.name || stand.standNo)}`;

    // Finansal kartlar
    document.getElementById('statTotalAmount').textContent = window.formatCurrency(financial.totalAmount);
    document.getElementById('statTotalPaid').textContent = window.formatCurrency(financial.totalPaid);
    document.getElementById('statBalance').textContent = window.formatCurrency(financial.balance);

    // Bilgi kartları
    populateStandInfo(stand);
    populateFairInfo(fair);
    populateParticipantInfo(participant);

    // Ödeme tablosu
    initializePaymentsTable(payments);
}

function populateStandInfo(stand) {
    document.getElementById('infoStandNo').textContent = window.escapeHtml(stand.name || stand.standNo);
    document.getElementById('infoArea').textContent = stand.contractArea || 0;
    document.getElementById('infoUnitPrice').textContent = window.formatCurrency(stand.unitPrice);
    document.getElementById('infoSalesPerson').textContent = window.escapeHtml(stand.salesRepresentative || '-');
    document.getElementById('infoStandNote').textContent = window.escapeHtml(stand.note || 'Girilmemiş.');
}

function populateFairInfo(fair) {
    if (!fair) return;
    const fairLink = document.getElementById('linkFairName');
    fairLink.textContent = window.escapeHtml(fair.name);
    fairLink.href = `fairDetails.html?id=${fair.id}`;
    document.getElementById('infoFairLocation').textContent = window.escapeHtml(fair.location || '-');
    document.getElementById('infoFairDates').textContent = `${window.formatDate(fair.startDate)} - ${window.formatDate(fair.endDate)}`;
}

function populateParticipantInfo(participant) {
    if (!participant) return;
    const participantLink = document.getElementById('linkParticipantName');
    participantLink.textContent = window.escapeHtml(participant.companyName);
    participantLink.href = `participantDetails.html?id=${participant.id}`;
    document.getElementById('infoParticipantContact').textContent = window.escapeHtml(participant.fullName || '-');
    document.getElementById('infoParticipantPhone').textContent = window.escapeHtml(participant.phone || '-');
}

/**
 * Ödemeler tablosunu DataTable ile başlatır.
 */
function initializePaymentsTable(payments) {
    $('#dataTablePayments').DataTable({
        data: payments,
        destroy: true,
        columns: [
            { title: 'Tarih', data: 'paymentDate', render: data => window.formatDate(data) },
            { title: 'Tutar', data: 'amount', render: (data, type, row) => window.formatCurrency(data || 0, row.currency) },
            { title: 'Ödeme Yöntemi', data: 'paymentMethod', defaultContent: '-' },
            { title: 'Açıklama', data: 'paymentDescription', defaultContent: '-' }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[0, 'desc']]
    });
}

/**
 * Olay dinleyicilerini ayarlar.
 */
function setupEventListeners(stand, participant) {
    const addPaymentModal = document.getElementById('addPaymentModal');
    addPaymentModal.addEventListener('show.bs.modal', () => {
        document.getElementById('paymentStandId').value = stand.id;
        document.getElementById('modalStandInfo').textContent = window.escapeHtml(stand.name || stand.standNo);
        document.getElementById('modalParticipantInfo').textContent = window.escapeHtml(participant?.companyName || '-');
        document.getElementById('addPaymentForm').reset();
        document.getElementById('paymentDate').valueAsDate = new Date();
        document.getElementById('receivedBy').value = localStorage.getItem('userName') || '';
    });

    document.getElementById('savePaymentButton').addEventListener('click', async () => {
        const paymentData = {
            standId: stand.id,
            paymentDate: document.getElementById('paymentDate').value,
            amount: parseFloat(document.getElementById('paymentAmount').value),
            currency: document.getElementById('paymentCurrency').value,
            paymentMethod: document.getElementById('paymentMethod').value,
            receivedBy: document.getElementById('receivedBy').value,
            paymentDescription: document.getElementById('paymentDescription').value,
        };

        if (isNaN(paymentData.amount) || !paymentData.paymentDate) {
            Swal.fire('Eksik Bilgi', 'Lütfen Tutar ve Tarih alanlarını doldurun.', 'warning');
            return;
        }

        try {
            await window.fetchWithToken('/Payments', {
                method: 'POST',
                body: JSON.stringify(paymentData)
            });
            Swal.fire('Başarılı!', 'Ödeme başarıyla eklendi.', 'success').then(() => {
                window.location.reload();
            });
        } catch (error) {
            Swal.fire('Hata!', `Ödeme eklenirken bir hata oluştu: ${error.message}`, 'error');
        }
    });
}
