library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_arith.ALL;
use IEEE.STD_LOGIC_unsigned.ALL;

entity LIFO is
	 Generic(
	 		p_DATA_WIDTH   : INTEGER := 16;        -- Número de bits dos dados. 
	 		p_ADD_WIDTH    : INTEGER := 9         -- Número de bits dos endereços. 
	 );
    Port ( 
		  i_CLK 	: in  STD_LOGIC;
		  i_RST   	: in  STD_LOGIC;
		  i_RD 		: in  STD_LOGIC;
		  i_WR 		: in  STD_LOGIC;
		  i_DATA  	: in  STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0);
		  o_DATA  	: out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0)
	 );
end LIFO;

architecture Behavioral of LIFO is

	COMPONENT MEMORIA is
		 Generic(
				p_DATA_WIDTH   : INTEGER := 16;        -- Número de bits dos dados. 
				p_ADD_WIDTH    : INTEGER := 9         -- Número de bits dos endereços. 
		 );
	    Port ( 
			  i_CLK 	: in  STD_LOGIC;
			  i_RST   	: in  STD_LOGIC;
			  i_WR 		: in  STD_LOGIC;
			  i_ADDR  	: in  STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0);
			  i_DATA  	: in  STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0);
			  o_DATA  	: out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0)
		 );
	end COMPONENT;

	type w_State_Type is (st_IDLE, st_WRITE, st_READ); 

	attribute syn_encoding : string;
	attribute syn_encoding of w_State_Type : type is "safe";
 
	signal w_STATE : w_State_Type;
	signal w_ADDR	: STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0);
	signal w_CLK    : STD_LOGIC;
	signal w_WR     : STD_LOGIC;
	
	----------------------------------------------------------------------------------------	
begin

	U_MACHINE : process(i_CLK, i_RST)          			
	begin    																						
		if (i_RST = '1') then			
			w_ADDR  <= (others=>'1');
			w_WR    <= '0';
			w_STATE	<= st_IDLE;				
			
		elsif rising_edge (i_CLK) then														
			case w_STATE is	
				when st_IDLE => 
					if (i_WR = '1') then
					    w_ADDR  <= w_ADDR + 1;
						w_WR    <= '1';
						w_STATE	<= st_WRITE;
					elsif ((i_RD = '1')  and ( w_ADDR /= "111111111")) then
					    w_STATE <= st_READ;
					else	
						w_STATE	<= st_IDLE;	
					end if;
					
				when st_WRITE =>
					w_WR <= '0';
					w_STATE	<= st_IDLE;
					
				when st_READ =>		
					w_ADDR <= w_ADDR - 1;
					w_STATE	<= st_IDLE;	
					
				when others => 																
					w_STATE <= st_IDLE;																				
			end case;																				

		end if;																						
	end process U_MACHINE;																	
		
	
	--
	-- Clock é invertido (falling_edge) para a memoria.
	--
	w_CLK <= NOT i_CLK;
	
	U_MEMO : MEMORIA 
		 Generic Map (
					p_DATA_WIDTH   => p_DATA_WIDTH,
					p_ADD_WIDTH    => p_ADD_WIDTH
	      )
	      Port Map ( 
					  i_CLK 	=> w_CLK,
					  i_RST   	=> i_RST,
					  i_WR 		=> w_WR,
					  i_ADDR  	=> w_ADDR,
					  i_DATA  	=> i_DATA,
					  o_DATA  	=> o_DATA 
		 );
		
		
end Behavioral;
