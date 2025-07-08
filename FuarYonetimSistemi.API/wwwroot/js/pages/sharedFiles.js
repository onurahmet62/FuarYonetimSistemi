// js/pages/sharedFiles.js

document.addEventListener('DOMContentLoaded', () => {
    window.loadSharedFilesPage = async function () {
        console.log("Ortak Dosyalar sayfası yükleniyor...");
        setupFileEventListeners();
        await loadFiles();
    };

    if (window.location.pathname.toLowerCase().includes('sharedfiles.html')) {
        window.loadSharedFilesPage();
    }
});

async function loadFiles() {
    try {
        const response = await window.fetchWithToken('/Files');
        const files = await response.json();
        renderFilesTable(window.processApiResponse(files));
    } catch (error) {
        console.error("Dosyalar yüklenirken hata:", error);
        document.getElementById('filesTableBody').innerHTML = `<tr><td colspan="5" class="text-center text-danger p-4">Dosyalar yüklenemedi.</td></tr>`;
    }
}

function renderFilesTable(files) {
    const tableBody = document.getElementById('filesTableBody');
    tableBody.innerHTML = '';

    if (!files || files.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center p-4">Paylaşılan dosya bulunmuyor.</td></tr>';
        return;
    }

    files.forEach(file => {
        const row = tableBody.insertRow();
        const fileSize = file.fileSize > 0 ? (file.fileSize / 1024 / 1024).toFixed(2) + ' MB' : '0 MB';
        const fileIcon = getFileIcon(file.fileType);

        row.innerHTML = `
            <td><i class="${fileIcon} me-2 file-icon"></i> ${window.escapeHtml(file.fileName)}</td>
            <td>${fileSize}</td>
            <td>${window.escapeHtml(file.uploadedByUserName || 'Bilinmiyor')}</td>
            <td>${window.formatDate(file.uploadTimestamp)}</td>
            <td>
                <a href="${API_BASE_URL}${file.filePath}" class="btn btn-sm btn-success" download="${window.escapeHtml(file.fileName)}" title="İndir"><i class="bi bi-download"></i></a>
                <button class="btn btn-sm btn-danger delete-file-btn" data-id="${file.id}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}

function getFileIcon(fileType) {
    if (!fileType) return 'bi bi-file-earmark';
    if (fileType.startsWith('image/')) return 'bi bi-file-earmark-image';
    if (fileType.includes('pdf')) return 'bi bi-file-earmark-pdf';
    if (fileType.includes('word')) return 'bi bi-file-earmark-word';
    if (fileType.includes('excel') || fileType.includes('spreadsheet')) return 'bi bi-file-earmark-excel';
    if (fileType.includes('presentation')) return 'bi bi-file-earmark-ppt';
    if (fileType.startsWith('text/')) return 'bi bi-file-earmark-text';
    return 'bi bi-file-earmark';
}

function setupFileEventListeners() {
    const browseButton = document.getElementById('browseButton');
    const fileInput = document.getElementById('fileInput');

    browseButton?.addEventListener('click', () => fileInput.click());
    fileInput?.addEventListener('change', handleFileUpload);

    document.getElementById('filesTableBody')?.addEventListener('click', (e) => {
        const deleteBtn = e.target.closest('.delete-file-btn');
        if (deleteBtn) confirmDeleteFile(deleteBtn.dataset.id);
    });
}

async function handleFileUpload(event) {
    const files = event.target.files;
    if (files.length === 0) return;

    const formData = new FormData();
    for (const file of files) {
        formData.append('files', file);
    }

    const progressBar = document.querySelector('#uploadProgress .progress-bar');
    const progressContainer = document.getElementById('uploadProgress');
    progressContainer.style.display = 'block';

    try {
        // fetchWithToken doğrudan FormData ile çalışmaz, headers'ı manuel ayarlamalıyız.
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE_URL}/Files/Upload`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}` }, // Content-Type'ı tarayıcı belirler
            body: formData,
            // Yükleme ilerlemesini izlemek için daha gelişmiş bir yapı (örn. XMLHttpRequest) gerekir.
            // Bu basit örnekte, sadece yükleme sonrası durumu ele alıyoruz.
        });

        if (!response.ok) {
            throw new Error('Dosya yüklenemedi.');
        }

        progressBar.style.width = '100%';
        progressBar.classList.add('bg-success');
        Swal.fire('Başarılı!', 'Dosya(lar) başarıyla yüklendi.', 'success');
        await loadFiles();

    } catch (error) {
        console.error("Dosya yükleme hatası:", error);
        progressBar.classList.add('bg-danger');
        Swal.fire('Hata!', 'Dosya yüklenirken bir sorun oluştu.', 'error');
    } finally {
        setTimeout(() => {
            progressContainer.style.display = 'none';
            progressBar.style.width = '0%';
            progressBar.classList.remove('bg-success', 'bg-danger');
        }, 2000);
    }
}

function confirmDeleteFile(id) {
    Swal.fire({
        title: 'Dosyayı Sil?', text: "Bu işlem geri alınamaz!", icon: 'warning', showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Evet, Sil!', cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Files/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Dosya başarıyla silindi.', 'success');
                await loadFiles();
            } catch (error) {
                console.error("Dosya silinirken hata:", error);
                Swal.fire('Hata!', 'Dosya silinirken bir sorun oluştu.', 'error');
            }
        }
    });
}
