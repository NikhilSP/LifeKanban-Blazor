window.navMenuDrag = {
    setup: function (dotNetRef) {
        console.log("Setting up nav menu drag and drop");
        const projectsList = document.querySelector('.projects-list');
        if (!projectsList) {
            console.error("Could not find projects-list element");
            return;
        }

        this.dotNetRef = dotNetRef;

        // Add event listeners to the container
        projectsList.addEventListener('dragstart', this.handleDragStart.bind(this));
        projectsList.addEventListener('dragover', this.handleDragOver.bind(this));
        projectsList.addEventListener('dragenter', this.handleDragEnter.bind(this));
        projectsList.addEventListener('dragleave', this.handleDragLeave.bind(this));
        projectsList.addEventListener('drop', this.handleDrop.bind(this));
        projectsList.addEventListener('dragend', this.handleDragEnd.bind(this));

        console.log("Nav menu drag-and-drop initialized successfully");
    },

    handleDragStart: function (e) {
        // Only process if the drag started on a project-item
        if (!e.target.classList.contains('project-item')) {
            const projectItem = e.target.closest('.project-item');
            if (!projectItem) return;

            // If we clicked a child element, redirect the drag to the parent
            e.dataTransfer.setDragImage(projectItem, 0, 0);
        }

        // Store the dragged item ID
        const projectItem = e.target.closest('.project-item');
        this.draggedId = projectItem.getAttribute('data-id');
        console.log("Drag started on item with ID:", this.draggedId);

        // Add a class to style the dragged item
        projectItem.classList.add('dragging');

        // Set required dataTransfer data
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', this.draggedId);
    },

    handleDragOver: function (e) {
        // Always prevent default to allow drop
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
    },

    handleDragEnter: function (e) {
        const item = e.target.closest('.project-item');
        if (!item) return;

        // Don't highlight the item we're dragging
        if (item.getAttribute('data-id') === this.draggedId) return;

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
        e.stopPropagation(); // Stop event from bubbling up

        console.log("Drop event triggered");

        // Find the drop target
        const dropTarget = e.target.closest('.project-item');
        if (!dropTarget) {
            console.log("No valid drop target found");
            return;
        }

        const dropTargetId = dropTarget.getAttribute('data-id');
        console.log("Drop target ID:", dropTargetId);

        // Prevent dropping onto itself
        if (this.draggedId === dropTargetId) {
            console.log("Dropped onto self, ignoring");
            return;
        }

        // Get all project items in order
        const projectItems = Array.from(document.querySelectorAll('.project-item'));

        // Find indexes
        const sourceIndex = projectItems.findIndex(item => item.getAttribute('data-id') === this.draggedId);
        const targetIndex = projectItems.indexOf(dropTarget);

        console.log("Source index:", sourceIndex, "Target index:", targetIndex);

        // Remove highlighting
        document.querySelectorAll('.drag-over').forEach(item => {
            item.classList.remove('drag-over');
        });

        // Call the .NET method to update positions
        console.log("Calling .NET method with", this.draggedId, targetIndex);
        this.dotNetRef.invokeMethodAsync('HandleProjectDrop', this.draggedId, targetIndex)
            .then(() => {
                console.log("HandleProjectDrop completed successfully");
            })
            .catch(error => {
                console.error("Error in HandleProjectDrop:", error);
            });
    },

    handleDragEnd: function (e) {
        // Clean up all styling classes
        document.querySelectorAll('.project-item').forEach(item => {
            item.classList.remove('dragging');
            item.classList.remove('drag-over');
        });

        console.log("Drag ended, cleanup complete");

        // Clear stored variables
        this.draggedId = null;
    }
};