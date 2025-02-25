document.addEventListener('DOMContentLoaded', function () {
    if (typeof resources === 'undefined' || Object.keys(resources).length === 0) {
        loadResourcesFromApi();
    } else {
        initializeUI();
    }
});

async function loadResourcesFromApi() {
    try {
        const response = await fetch('/api/Resources');
        if (response.ok) {
            window.resources = await response.json();
            initializeUI();
        } else {
            initializeUI();
        }
    } catch (error) {
        initializeUI();
    }
}

function R(key) {
    if (typeof resources !== 'undefined' && resources[key]) {
        return resources[key];
    }
    return key;
}

function initializeUI() {
    setupRowSelection();
    setupModalHandling();
    setupDeleteConfirmation();
}

function setupRowSelection() {
    const rows = document.querySelectorAll('.settings-row');
    const editButton = document.getElementById('editButton');
    const deleteButton = document.getElementById('deleteButton');
    let selectedEntityId = null;

    rows.forEach(row => {
        row.addEventListener('click', function () {
            rows.forEach(r => r.classList.remove('selected'));
            this.classList.add('selected');
            selectedEntityId = this.getAttribute('data-id');

            if (editButton) {
                editButton.disabled = false;
                editButton.setAttribute('data-id', selectedEntityId);
            }

            if (deleteButton) {
                deleteButton.disabled = false;
                deleteButton.setAttribute('data-id', selectedEntityId);
            }
        });
    });
}

function setupModalHandling() {
    const modal = document.getElementById('entityModal');
    if (!modal) { return; }

    modal.addEventListener('show.bs.modal', async function (event) {
        try {
            const button = event.relatedTarget;
            if (!button) return;
            
            const entity = button.getAttribute('data-entity');
            const formMode = button.getAttribute('data-whatever');
            const entityId = button.getAttribute('data-id');
            
            const saveBtn = document.getElementById('SaveBtn');
            const addBtn = document.getElementById('AddBtn');
            
            if (saveBtn) saveBtn.textContent = R('SaveBtn');
            if (addBtn) addBtn.textContent = R('AddBtn');

            const entityUrl = entity.charAt(0).toLowerCase() + entity.slice(1);

            const response = await fetch(`/${entityUrl}/GetForm`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    mode: formMode,
                    id: entityId ? parseInt(entityId, 10) : null
                })
            });

            if (!response.ok) {
                throw new Error(`${response.status}: ${await response.text()}`);
            }

            const formHtml = await response.text();

            const titleElement = document.getElementById('modalTitle');
            if (titleElement) {
                titleElement.textContent = `${formMode.charAt(0).toUpperCase() + formMode.slice(1)} ${entity.replace(/s$/, '')}`;
            }

            const bodyElement = document.getElementById('modalBody');
            if (bodyElement) {
                bodyElement.innerHTML = formHtml;
            } else {
                throw new Error('Element modalBody not found');
            }

            const submitBtn = document.getElementById('submitBtn');
            if (submitBtn) {
                submitBtn.textContent = formMode === 'edit' ? R('SaveBtn') : R('AddBtn');
            }

            if (formMode === 'edit' && entityId) {
                await loadEntityData(entityUrl, entityId);
            }

            setupFormHandling(formMode, entityUrl, entityId, modal);

        } catch (error) {
            alert(error.message);
        }
    });
}

async function loadEntityData(entityUrl, entityId) {
    try {
        const dataResponse = await fetch(`/${entityUrl}/GetEntity?id=${entityId}`);
        if (!dataResponse.ok) {
            throw new Error(dataResponse.status);
        }

        const data = await dataResponse.json();

        const inputs = document.querySelectorAll('#modalBody input, #modalBody select');
        inputs.forEach(input => {
            if (input.name in data) {
                input.value = data[input.name];
            }
        });
    } catch (error) {
        alert(error.message);
    }
}

function setupFormHandling(formMode, entityUrl, entityId, modal) {
    const inputs = document.querySelectorAll('#modalBody input, #modalBody select');
    const submitBtn = document.getElementById('submitBtn');

    if (!submitBtn) { return; }

    const validateForm = () => {
        const isFormValid = [...inputs].every(input => {
            return !input.hasAttribute('required') || input.value.trim() !== '';
        });
        submitBtn.disabled = !isFormValid;
    };

    inputs.forEach(input => input.addEventListener('input', validateForm));
    validateForm();

    submitBtn.onclick = async () => {
        try {
            const formData = new FormData();
            inputs.forEach(input => formData.append(input.name, input.value.trim()));

            const url = formMode === 'edit'
                ? `/${entityUrl}/Update?id=${entityId}`
                : `/${entityUrl}/Create`;

            const submitResponse = await fetch(url, {
                method: 'POST',
                body: formData
            });

            if (!submitResponse.ok) {
                const errorText = await submitResponse.text();
                throw new Error(errorText);
            }

            const modalInstance = bootstrap.Modal.getInstance(modal);
            if (modalInstance) {
                modalInstance.hide();
            }
            location.reload();

        } catch (error) {
            alert(error.message);
        }
    };
}

function setupDeleteConfirmation() {
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
    const confirmDeleteModal = document.getElementById('confirmDeleteModal');

    if (confirmDeleteBtn && confirmDeleteModal) {
        confirmDeleteBtn.addEventListener('click', async function () {
            try {
                const deleteButton = document.getElementById('deleteButton');
                const entityId = deleteButton.getAttribute('data-id');
                const entity = deleteButton.getAttribute('data-entity');
                const entityUrl = entity.charAt(0).toLowerCase() + entity.slice(1);

                const response = await fetch(`/${entityUrl}/Delete?id=${entityId}`, {
                    method: 'POST'
                });

                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }

                const modalInstance = bootstrap.Modal.getInstance(confirmDeleteModal);
                if (modalInstance) {
                    modalInstance.hide();
                }
                location.reload();

            } catch (error) {
                alert(error.message);
            }
        });
    }
}