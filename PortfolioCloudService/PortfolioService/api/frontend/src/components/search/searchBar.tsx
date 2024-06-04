import React, { useState } from 'react';
import Select from 'react-select';

interface AutocompleteDropdownProps {
  options: SelectOption[];
}

interface SelectOption {
  label: string;
  value: string;
}

const AutocompleteDropdown: React.FC<AutocompleteDropdownProps> = ({ options }) => {
  const [selectedOption, setSelectedOption] = useState(null);

  const handleChange = (selected: any) => {
    setSelectedOption(selected);
  };

  return (
    <div style={{marginTop: 10, border: '1px solid #257953', borderRadius: 5}}>
      <Select
        value={selectedOption}
        onChange={handleChange}
        options={options}
        isSearchable
        placeholder="Search for crypto currencies..."
      />
    </div>
  );
};

export default AutocompleteDropdown;