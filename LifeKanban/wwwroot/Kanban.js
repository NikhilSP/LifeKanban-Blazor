window.kanbanInterop = {
    // Handle both prevent default and insertion line in one function
    handleDragOver: function(event, columnId, dotNetHelper) {
        // Prevent default browser behavior
        event.preventDefault();

        console.log(`Handling drag over column ${columnId}`);

        // Show insertion line and get index
        const insertIndex = this.showInsertionLine(columnId, event.clientY);

        // Notify Blazor of the index (optional)
        if (dotNetHelper) {
            dotNetHelper.invokeMethodAsync('OnInsertIndexChanged', insertIndex);
        }

        return false; // Prevent default
    },

    // Original function for showing insertion line
    showInsertionLine: function(columnId, mouseY) {
        try {
            // Remove existing indicators
            this.removeInsertionLine();

            // Find column container
            const column = document.querySelector(`.column-${columnId} .cards-container`);
            if (!column) return -1;

            // Find all cards in the column
            const cards = column.querySelectorAll('.kanban-card');
            let insertIndex = cards.length; // Default to end

            if (cards.length === 0) {
                // Empty column
                this.createIndicator(column, true);
                return 0;
            }

            // Find insertion point
            for (let i = 0; i < cards.length; i++) {
                const rect = cards[i].getBoundingClientRect();
                const middle = rect.top + rect.height / 2;

                if (mouseY < middle) {
                    this.createIndicator(column, false, cards[i]);
                    insertIndex = i;
                    break;
                }
            }

            // If we didn't insert before any card, append at the end
            if (insertIndex === cards.length) {
                this.createIndicator(column, false);
            }

            return insertIndex;
        } catch (error) {
            console.error("Error in showInsertionLine:", error);
            return -1;
        }
    },

    removeInsertionLine: function() {
        const indicator = document.getElementById('insertion-indicator');
        if (indicator) indicator.remove();
    },

    createIndicator: function(container, isEmpty, beforeElement = null) {
        const indicator = document.createElement('div');
        indicator.id = 'insertion-indicator';
        indicator.style.height = '4px';
        indicator.style.backgroundColor = '#009966';
        indicator.style.margin = '8px 0';

        if (beforeElement) {
            container.insertBefore(indicator, beforeElement);
        } else {
            container.appendChild(indicator);
        }
    }
};