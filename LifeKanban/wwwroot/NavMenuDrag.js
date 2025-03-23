window.navMenuDrag = {
    setup: function (dotNetRef) {
        const projectsList = document.querySelector('.projects-list');
        if (!projectsList) return;

        this.dotNetRef = dotNetRef;

        // Add event listeners to make the list sortable
        projectsList.addEventListener('dragstart', this.handleDragStart.bind(this));
        projectsList.addEventListener('dragover', this.handleDragOver.bind(this));
        projectsList.addEventListener('dragenter', this.handleDragEnter.bind(this));
        projectsList.addEventListener('dragleave', this.handleDragLeave.bind(this));
        projectsList.addEventListener('drop', this.handleDrop.bind(this));
        projectsList.addEventListener('dragend', this.handleDragEnd.bind(this));

        console.log("Nav menu drag-and-drop initialized");
    },

    handleDragStart: function (e) {
        if (!e.target.classList.contains('project-item')) return;

        // Store the dragged item ID and initial index
        this.draggedId = e.target.getAttribute('data-id');
        this.dragSourceIndex = Array.from(e.target.parentNode.children).indexOf(e.target);

        // Add a class to style the dragged item
        e.target.classList.add('dragging');

        // Set required dataTransfer data
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', this.draggedId);

        // Use a custom ghost image (optional)
        const ghost = e.target.cloneNode(true);
        ghost.style.position = 'absolute';
        ghost.style.top = '-1000px';
        ghost.style.opacity = '0.8';
        document.body.appendChild(ghost);
        e.dataTransfer.setDragImage(ghost, 0, 0);

        // Remove the ghost element after a delay
        setTimeout(() => {
            document.body.removeChild(ghost);
        }, 0);
    },

    handleDragOver: function (e) {
        // The current implementation only prevents default when hovering over a project item
        // We need to prevent default for the entire projects-list area
        e.preventDefault(); // Always prevent default to indicate dropping is allowed
        e.dataTransfer.dropEffect = 'move';

        // The rest of the logic can stay the same
        if (e.target.closest('.project-item')) {
            // Logic for when hovering over a specific project item
        }
    },

    handleDragEnter: function (e) {
        const item = e.target.closest('.project-item');
        if (!item) return;

        // Highlight drop target when dragged item enters it
        item.classList.add('drag-over');
    },

    handleDragLeave: function (e) {
        const item = e.target.closest('.project-item');
        if (!item) return;

        // Remove highlight when dragged item leaves
        item.classList.remove('drag-over');
    },

    handleDrop: function (e) {
        e.preventDefault();

        // Find the drop target
        const dropTarget = e.target.closest('.project-item');
        if (!dropTarget || this.draggedId === dropTarget.getAttribute('data-id')) return;

        // Get the drop target index
        const dropTargetIndex = Array.from(dropTarget.parentNode.children).indexOf(dropTarget);

        // Remove highlighting
        dropTarget.classList.remove('drag-over');

        // Calculate new positions
        const sourceId = this.draggedId;
        const targetIndex = dropTargetIndex;

        // Call the .NET method to update positions
        this.dotNetRef.invokeMethodAsync('HandleProjectDrop', sourceId, targetIndex);
    },

    handleDragEnd: function (e) {
        // Clean up
        const items = document.querySelectorAll('.project-item');
        items.forEach(item => {
            item.classList.remove('dragging');
            item.classList.remove('drag-over');
        });

        // Clear stored variables
        this.draggedId = null;
        this.dragSourceIndex = null;
    }
};