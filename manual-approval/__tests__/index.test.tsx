import { render, screen } from '@testing-library/react'
import Home from '@/pages/index';

describe('Home', () => {
    it('renders a title', () => {
        render(<Home />);

        const t3Title = screen.getByTestId('create-t3-title');

        expect(t3Title).toBeInTheDocument();
    })
})