package com.mycompany.customerapi.mapper;

import com.mycompany.customerapi.model.Customer;
import com.mycompany.customerapi.rest.dto.CreateCustomerRequest;
import com.mycompany.customerapi.rest.dto.CustomerDto;
import com.mycompany.customerapi.rest.dto.UpdateCustomerRequest;
import javax.annotation.processing.Generated;
import org.springframework.stereotype.Component;

@Generated(
    value = "org.mapstruct.ap.MappingProcessor",
    date = "2020-11-08T20:20:09+0200",
    comments = "version: 1.3.1.Final, compiler: javac, environment: Java 11.0.9 (Ubuntu)"
)
@Component
public class CustomerMapperImpl implements CustomerMapper {

    @Override
    public CustomerDto toCustomerDto(Customer customer) {
        if ( customer == null ) {
            return null;
        }

        CustomerDto customerDto = new CustomerDto();

        customerDto.setId( customer.getId() );
        customerDto.setFirstName( customer.getFirstName() );
        customerDto.setLastName( customer.getLastName() );

        return customerDto;
    }

    @Override
    public Customer toCustomer(CreateCustomerRequest createCustomerRequest) {
        if ( createCustomerRequest == null ) {
            return null;
        }

        Customer customer = new Customer();

        customer.setFirstName( createCustomerRequest.getFirstName() );
        customer.setLastName( createCustomerRequest.getLastName() );

        return customer;
    }

    @Override
    public void updateCustomerFromDto(UpdateCustomerRequest updateCustomerRequest, Customer customer) {
        if ( updateCustomerRequest == null ) {
            return;
        }

        if ( updateCustomerRequest.getFirstName() != null ) {
            customer.setFirstName( updateCustomerRequest.getFirstName() );
        }
        if ( updateCustomerRequest.getLastName() != null ) {
            customer.setLastName( updateCustomerRequest.getLastName() );
        }
    }
}
