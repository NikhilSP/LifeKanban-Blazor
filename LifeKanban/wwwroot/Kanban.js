window.kanbanInterop = {

    setDotNetReference: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        console.log("DotNet reference set");
    },

    // Handle both prevent default and insertion line in one function
    handleDragOver: function (event, columnId) {
        event.preventDefault();

        // Get card positions
        const column = document.querySelector(`[data-column-id="${columnId}"] .cards-container`);
        if (!column) return false;

        // Find all cards
        const cards = Array.from(column.querySelectorAll('.kanban-card'));
        let insertIndex = cards.length;

        // Remove existing indicator
        this.removeInsertionLine();

        // Find insertion point
        for (let i = 0; i < cards.length; i++) {
            const rect = cards[i].getBoundingClientRect();
            const middle = rect.top + rect.height / 2;

            if (event.clientY < middle) {
                // Insert before this card
                this.createIndicator(column, false, cards[i]);
                insertIndex = i;
                break;
            }
        }

        // If no insertion point found, append at end
        if (insertIndex === cards.length) {
            this.createIndicator(column, false);
        }

        // Tell Blazor about the insertion index
        if (this._dotNetRef) {
            this._dotNetRef.invokeMethodAsync('UpdateInsertIndex', insertIndex);
        }

        return false;
    },

    // Original function for showing insertion line
    showInsertionLine: function (columnId, mouseY) {
        try {
            console.log(`DEBUG: showInsertionLine called with column ${columnId}, mouseY ${mouseY}`);

            // Remove existing indicator
            this.removeInsertionLine();

            // Find column container
            const column = document.querySelector(`[data-column-id="${columnId}"] .cards-container`);

            if (!column) {
                console.error(`Column not found for ID ${columnId}`);
                return -1;
            }

            console.log("Column found, getting cards");

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

    removeInsertionLine: function () {
        const indicator = document.getElementById('insertion-indicator');
        if (indicator) indicator.remove();
    },

    createIndicator: function (container, isEmpty, beforeElement = null) {
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
    },

    clearDotNetReference: function() {
        this._dotNetRef = null;
        console.log("DotNet reference cleared");
    }
};